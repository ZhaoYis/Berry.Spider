using System.Text;

namespace Berry.Spider.Core;

public interface IStringBuilderObjectPoolProvider
{
    string Invoke(Action<StringBuilder> build, int initialCapacity = 256, int maximumRetainedCapacity = 8192);
}