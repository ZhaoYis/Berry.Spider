namespace Berry.Spider.Core;

/// <summary>
/// Transformer之Self-Attention自注意力机制实现
/// 参考资料：
/// https://zhuanlan.zhihu.com/p/455399791?utm_id=0
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
        n = sequenceLength; // 设置序列长度
        d = embeddingDimension; // 设置嵌入维度
        d_k = keyDimension; // 设置Q和K的维度
        d_v = valueDimension; // 设置V的维度
    }

    /// <summary>
    /// 计算自注意力机制的输出
    /// </summary>
    /// <param name="X">输入的嵌入矩阵</param>
    /// <param name="W_Q">计算Q的权重矩阵</param>
    /// <param name="W_K">计算K的权重矩阵</param>
    /// <param name="W_V">计算V得权重矩阵</param>
    /// <returns></returns>
    public double[,] ComputeAttention(double[,] X, double[,] W_Q, double[,] W_K, double[,] W_V)
    {
        // 计算Q、K和V矩阵
        double[,] Q = MatrixMultiply(X, W_Q); // 计算查询矩阵 Q = X * W_Q
        double[,] K = MatrixMultiply(X, W_K); // 计算键矩阵 K = X * W_K
        double[,] V = MatrixMultiply(X, W_V); // 计算值矩阵 V = X * W_V

        // 计算K的转置矩阵，主要目的是获得注意力得分
        // 具体来说，Q 和 K 的点积反映了输入序列中各个元素之间的相似性或关联程度。
        // 通过将 K 转置后，可以将 Q 和 K 的每一对元素进行比较，从而计算出相似度矩阵
        double[,] K_T = MatrixTranspose(K); // K^T

        // 计算Q与K的点积，得到相似度
        double[,] QK_T = MatrixMultiply(Q, K_T); // QK^T

        // 计算缩放因子sqrt(d_k)，即对Q、K的维度d_k开根号
        double scale = Math.Sqrt(d_k); // sqrt(d_k)

        // 将相似度得分QK_T除以缩放因子，以减小数值范围
        // 除以缩放因子的目的是为了减小数值范围，从而避免在计算 Softmax 时可能出现的数值不稳定性。
        // 具体来说，当输入向量的维度较高时，点积的结果可能会变得非常大，
        // 这可能导致 Softmax 函数中的指数计算产生极大的值，从而引起梯度消失或数值溢出的问题
        double[,] scaledScores = ScalarDivide(QK_T, scale); // QK^T / sqrt(d_k)

        // 应用Softmax函数得到注意力权重（或者叫attention score）
        double[,] attentionWeights = Softmax(scaledScores); // Softmax(QK^T / sqrt(d_k))

        // 计算注意力
        return MatrixMultiply(attentionWeights, V); // 注意力输出 = 权重 * V
    }

    /// <summary>
    /// 矩阵相乘的辅助方法，即A的行与B的列的点积
    /// </summary>
    /// <returns></returns>
    private double[,] MatrixMultiply(double[,] A, double[,] B)
    {
        int m = A.GetLength(0); // A的行数
        int p = A.GetLength(1); // A的列数（也是B的行数）
        int n = B.GetLength(1); // B的列数

        double[,] C = new double[m, n]; // 结果矩阵C

        // 遍历矩阵A的每一行
        for (int i = 0; i < m; i++)
        {
            // 遍历矩阵B的每一列
            for (int j = 0; j < n; j++)
            {
                double sum = 0.0; // 初始化和为0
                // 计算A的行和B的列的点积
                for (int k = 0; k < p; k++)
                {
                    sum += A[i, k] * B[k, j]; // 累加乘积
                }

                C[i, j] = sum; // 将结果存入C矩阵
            }
        }

        return C; // 返回结果矩阵C
    }

    /// <summary>
    /// 矩阵转置的辅助方法。
    /// 矩阵转置是线性代数中的一个操作，它指的是将一个矩阵的行和列互换
    /// </summary>
    /// <param name="A">目标矩阵</param>
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

        return A_T; // 返回转置后的矩阵
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

        return result; // 返回结果矩阵
    }

    /// <summary>
    /// Softmax函数的辅助方法，用于将得分转换为概率分布
    /// </summary>
    /// <returns></returns>
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

        return softmax; // 返回Softmax结果矩阵
    }
}