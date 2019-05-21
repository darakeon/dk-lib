namespace DK.NHibernate.Base
{
	internal interface ITransactionController
	{
		void Begin();
		void Commit();
		void Rollback();
	}
}
