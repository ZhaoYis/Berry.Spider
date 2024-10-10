using System;
using System.Text;
using Berry.Spider.Core;
using Xunit;
using Xunit.Abstractions;

public class SelfAttention_UnitTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public SelfAttention_UnitTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void ComputeAttention_Test()
    {
        // 输入嵌入矩阵 X（例如这里模拟3个词）,实际可以使用word embdding来生成
        double[,] X = new double[,]
        {
            { 1.0, 0.0, 1.0, 0.0 }, //我
            { 0.0, 1.0, 0.0, 1.0 }, //爱
            { 1.0, 1.0, 1.0, 1.0 }, //你
        };

        // 权重矩阵 W_Q, W_K, W_V
        // 可以是固定的权重矩阵，也可以通过训练得到，有点像卷积神经网络的卷积核
        double[,] W_Q = RandomMatrix(4, 4);
        double[,] W_K = RandomMatrix(4, 4);
        double[,] W_V = RandomMatrix(4, 4);

        // 输入参数
        int n = X.GetLength(0); // 序列长度（例如3个词）
        int d = X.GetLength(1); // 嵌入维度
        int d_k = W_K.GetLength(1); // Q、K 的维度
        int d_v = W_V.GetLength(1); // V 的维度

        // 创建实例
        var selfAttention = new SelfAttention(sequenceLength: n, embeddingDimension: d, keyDimension: d_k, valueDimension: d_v);

        // 计算注意力输出
        double[,] output = selfAttention.ComputeAttention(X, W_Q, W_K, W_V);

        // 验证输出的维度
        Assert.Equal(n, output.GetLength(0));
        Assert.Equal(d_v, output.GetLength(1));

        // 打印输出
        PrintMatrix(output);
    }

    /// <summary>
    /// 随机生成指定长、宽的矩阵
    /// </summary>
    /// <returns></returns>
    private double[,] RandomMatrix(int rows, int cols)
    {
        double[,] matrix = new double[rows, cols];
        Random rnd = new Random();
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = rnd.NextDouble();
            }
        }

        return matrix;
    }

    /// <summary>
    /// 辅助方法用于打印矩阵
    /// </summary>
    /// <param name="matrix"></param>
    private void PrintMatrix(double[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < rows; i++)
        {
            stringBuilder.Append("[");

            for (int j = 0; j < cols; j++)
            {
                stringBuilder.Append($"{matrix[i, j]},");
            }

            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("]");

            _testOutputHelper.WriteLine(stringBuilder.ToString());
            stringBuilder.Clear();
        }
    }
}