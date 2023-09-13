namespace CardboardCore.Pooling
{
	public interface IPoolable
	{
		void OnPop();
		void OnPush();
	}
}
