namespace Keon.NHibernate.Sessions
{
	internal interface ITransactionController
	{
		void Begin();
		void Commit();
		void Rollback();
	}
}
