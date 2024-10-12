namespace Berry.Spider.Core;

/// <summary>
/// Transformer之Self-Attention自注意力机制实现
/// 参考资料：
/// https://zhuanlan.zhihu.com/p/455399791
/// https://zhuanlan.zhihu.com/p/410776234
/// https://blog.csdn.net/weixin_45303602/article/details/134188049
/// </summary>
public class SelfAttention
{
    private readonly int n; // 序列长度
    private readonly int d; // 嵌入维度
    private readonly int d_k; // Q、K 的维度
    private readonly int d_v; // V 的维度

    /// <summary>
    /// 初始化自注意力机制的参数
    /// </summary>
    /// <param name="sequenceLength">序列长度</param>
    /// <param name="embeddingDimension">嵌入维度</param>
    /// <param name="keyDimension">Q和K的维度</param>
    /// <param name="valueDimension">V的维度</param>
    public SelfAttention(int sequenceLength, int embeddingDimension, int keyDimension, int valueDimension)
    {
        n = sequenceLength;
        d = embeddingDimension;
        d_k = keyDimension;
        d_v = valueDimension;
    }

    /// <summary>
    /// 计算自注意力
    /// </summary>
    /// <param name="X">输入的嵌入矩阵</param>
    /// <param name="W_Q">计算Q的权重矩阵</param>
    /// <param name="W_K">计算K的权重矩阵</param>
    /// <param name="W_V">计算V得权重矩阵</param>
    /// <returns></returns>
    public double[,] ComputeSelfAttention(double[,] X, double[,] W_Q, double[,] W_K, double[,] W_V)
    {
        // 1、添加位置编码到输入嵌入矩阵
        double[,] X_pos = AddPositionalEncoding(X); // X' = X + PE

        // 2、计算Q、K和V矩阵
        double[,] Q = MatrixMultiply(X_pos, W_Q); // 计算查询矩阵 Q = X * W_Q
        double[,] K = MatrixMultiply(X_pos, W_K); // 计算键矩阵 K = X * W_K
        double[,] V = MatrixMultiply(X_pos, W_V); // 计算值矩阵 V = X * W_V

        // 3、计算K的转置矩阵
        // 主要目的是获得注意力得分，具体来说，Q 和 K 的点积（几何含义就是K在Q上的投影，投影的值越大说明两个向量相关度高，90度表明二者毫无关系）反映了输入序列中各个元素之间的相似性或关联程度。
        // 通过将 K 转置后，可以将 Q 和 K 的每一对元素进行比较，从而计算出相似度矩阵
        double[,] K_T = MatrixTranspose(K); // K^T

        // 4、计算Q与K_T的点积，得到相似度
        double[,] QK_T = MatrixMultiply(Q, K_T); // QK_T = Q * K_T

        // 5、计算缩放因子sqrt(d_k)，即对Q、K的维度d_k开根号
        double scale = Math.Sqrt(d_k); // sqrt(d_k)

        // 6、将相似度得分QK_T除以缩放因子（transformer中使用sqrt(d_k)作为缩放因子）
        // 除以缩放因子的目的是为了减小数值范围，从而避免在计算 Softmax 时可能出现的数值不稳定性。
        // 具体来说，当输入向量的维度较高时，点积的结果可能会变得非常大，这可能导致 Softmax 函数中的指数计算产生极大的值，从而引起梯度消失或梯度爆炸的问题，
        // 梯度消失会导致深层网络前面的层权值几乎不变，仍接近于初始化的权值，就等价于只有后几层的浅层网络的学习了。
        // 梯度消失问题和梯度爆炸问题一般随着网络层数的增加会变得越来越明显，因此需要进行缩放。
        double[,] QK_T_scaled = ScalarDivide(QK_T, scale); // QK^T / sqrt(d_k)

        // 7、应用Softmax函数得到注意力权重（或者叫attention scores）
        // 在softmax之后，attention score矩阵的每一行表示一个token，每一列表示该token和对应位置token的α值，
        // 因为进行了softmax，所以每一行的α值相加等于1
        double[,] attentionWeights = Softmax(QK_T_scaled); // Softmax(QK^T / sqrt(d_k))

        // 8、计算注意力
        return MatrixMultiply(attentionWeights, V); // Attention Output = Attention Weights * V
    }

    /// <summary>
    /// 为输入嵌入矩阵添加正弦和余弦位置编码
    /// </summary>
    /// <param name="X">输入嵌入矩阵 (n x d)</param>
    /// <returns>添加位置编码后的矩阵 (n x d)</returns>
    private double[,] AddPositionalEncoding(double[,] X)
    {
        // 初始化位置编码矩阵
        double[,] PE = GeneratePositionalEncoding(sequenceLength: n, embeddingDimension: d);

        // 创建一个新的矩阵用于存储添加位置编码后的结果
        double[,] X_pos = new double[n, d];

        // 将输入矩阵 X 与位置编码矩阵 PE 相加
        for (int i = 0; i < n; i++) // 遍历每个位置
        {
            for (int j = 0; j < d; j++) // 遍历每个维度
            {
                X_pos[i, j] = X[i, j] + PE[i, j]; // X' = X + PE
            }
        }

        return X_pos;
    }

    /// <summary>
    /// 生成正弦和余弦位置编码矩阵
    /// </summary>
    /// <param name="sequenceLength">序列长度</param>
    /// <param name="embeddingDimension">嵌入维度</param>
    /// <returns>位置编码矩阵 (n x d)</returns>
    private double[,] GeneratePositionalEncoding(int sequenceLength, int embeddingDimension)
    {
        double[,] PE = new double[sequenceLength, embeddingDimension]; // 创建位置编码矩阵

        // 计算位置编码
        for (int pos = 0; pos < sequenceLength; pos++) // 遍历每个位置
        {
            for (int i = 0; i < embeddingDimension; i++) // 遍历每个维度
            {
                if (i % 2 == 0)
                {
                    // 偶数维度使用正弦函数
                    PE[pos, i] = Math.Sin(pos / Math.Pow(10000, (double)i / embeddingDimension));
                }
                else
                {
                    // 奇数维度使用余弦函数
                    PE[pos, i] = Math.Cos(pos / Math.Pow(10000, (double)(i - 1) / embeddingDimension));
                }
            }
        }

        return PE;
    }

    /// <summary>
    /// 矩阵相乘的辅助方法，即A的行与B的列的点积
    /// </summary>
    /// <returns></returns>
    private double[,] MatrixMultiply(double[,] A, double[,] B)
    {
        int A_rows = A.GetLength(0); // A的行数
        int A_cols = A.GetLength(1); // A的列数（也是B的行数）
        int B_rows = B.GetLength(1); // B的行数
        int B_cols = B.GetLength(1); // B的列数

        // 确保维度匹配
        if (A_cols != B_rows)
            throw new ArgumentException("矩阵A的列数必须等于矩阵B的行数");

        double[,] C = new double[A_rows, B_cols]; // 结果矩阵C

        // 遍历矩阵A的每一行
        for (int i = 0; i < A_rows; i++)
        {
            // 遍历矩阵B的每一列
            for (int j = 0; j < B_cols; j++)
            {
                double sum = 0.0; // 初始化和为0
                // 计算A的行和B的列的点积
                for (int k = 0; k < A_cols; k++)
                {
                    sum += A[i, k] * B[k, j]; // 累加乘积
                }

                C[i, j] = sum; // 将结果存入C矩阵
            }
        }

        return C;
    }

    /// <summary>
    /// 矩阵转置的辅助方法。
    /// 矩阵转置是线性代数中的一个操作，它指的是将一个矩阵的行和列互换
    /// </summary>
    /// <returns></returns>
    private double[,] MatrixTranspose(double[,] A)
    {
        int rows = A.GetLength(0); // A的行数
        int cols = A.GetLength(1); // A的列数
        double[,] A_T = new double[cols, rows]; // 创建转置后的矩阵

        // 遍历矩阵A进行转置
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            A_T[j, i] = A[i, j]; // 交换行列

        return A_T;
    }

    /// <summary>
    /// 矩阵除以标量的辅助方法
    /// </summary>
    /// <returns></returns>
    private double[,] ScalarDivide(double[,] A, double s)
    {
        int rows = A.GetLength(0); // A的行数
        int cols = A.GetLength(1); // A的列数
        double[,] result = new double[rows, cols]; // 创建结果矩阵

        // 遍历矩阵A进行标量除法
        for (int i = 0; i < rows; i++)
        for (int j = 0; j < cols; j++)
            result[i, j] = A[i, j] / s; // 每个元素除以s

        return result;
    }

    /// <summary>
    /// Softmax函数，用于将得分转换为概率分布
    /// </summary>
    /// <returns>Softmax结果矩阵</returns>
    private double[,] Softmax(double[,] A)
    {
        int rows = A.GetLength(0); // A的行数
        int cols = A.GetLength(1); // A的列数
        double[,] softmax = new double[rows, cols]; // 创建Softmax结果矩阵

        // 遍历每一行计算Softmax
        for (int i = 0; i < rows; i++)
        {
            double max = A[i, 0]; // 找到行的最大值
            // 找到当前行的最大值
            for (int j = 1; j < cols; j++)
                if (A[i, j] > max)
                    max = A[i, j]; // 更新最大值

            double sum = 0.0; // 初始化和为0
            double[] exps = new double[cols]; // 创建存储指数的数组

            // 计算指数并求和
            for (int j = 0; j < cols; j++)
            {
                exps[j] = Math.Exp(A[i, j] - max); // 计算e^(A[i,j] - max)
                sum += exps[j]; // 累加指数
            }

            // 计算Softmax结果
            for (int j = 0; j < cols; j++)
            {
                softmax[i, j] = exps[j] / sum; // 每个指数除以总和
            }
        }

        return softmax;
    }
}