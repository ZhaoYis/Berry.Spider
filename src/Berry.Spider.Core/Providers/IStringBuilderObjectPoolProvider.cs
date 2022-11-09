using System.Text;

namespace Berry.Spider.Core;

public interface IStringBuilderObjectPoolProvider
{
    string Invoke(Action<StringBuilder> build, int initialCapacity, int maximumRetainedCapacity);

    string Invoke(Action<StringBuilder> build);
}