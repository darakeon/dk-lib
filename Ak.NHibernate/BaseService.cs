using System;
using Ak.Generic.Exceptions;

namespace Ak.NHibernate
{
    /// <summary>
    /// Base for services, to have transaction
    /// </summary>
    public class BaseService
    {
        /// <summary>
        /// 
        /// </summary>
        public BaseService()
        {
            TransactionController = new TransactionController();
        }

        /// <summary>
        /// To control DB Transaction
        /// </summary>
        internal TransactionController TransactionController { get; private set; }



        /// <summary>
        /// Starts Transaction
        /// </summary>
        /// <exception cref="AkException">Transaction already opened</exception>
        protected void BeginTransaction()
        {
            TransactionController.Begin();
        }

        /// <summary>
        /// Execute everything at DB
        /// </summary>
        protected void CommitTransaction()
        {
            try
            {
                TransactionController.Commit();
            }
            catch (Exception e)
            {
                AkException.TestOtherIfTooLarge(e);
            }
        }

        /// <summary>
        /// Undo all changes
        /// </summary>
        protected void RollbackTransaction()
        {
            TransactionController.Rollback();
        }





    }
}
