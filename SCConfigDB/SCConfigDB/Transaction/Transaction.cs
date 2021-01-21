using System;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public abstract class Transaction : ITransaction
    {
        public bool Applied { get; protected set; }
        public bool Commited { get; protected set; }
        protected abstract bool OnApply();
        protected abstract void OnRevert();
        protected abstract void OnCommit();

        public void Dispose()
        {
            if (!Commited)
            {
                if (Applied)
                {
                    OnRevert();
                    Applied = false;
                }
                Commited = true;
                OnCommit();
            }
        }

        public bool Apply()
        {
            if (Commited)
                throw new InvalidOperationException("Can't apply after commit transation");
            if (!Applied && OnApply())
            {
                Applied = true;
            }
            return Applied;
        }

        public void Revert()
        {
            if (Commited)
                throw new InvalidOperationException("Can't revert after commit transation");
            if (Applied)
            {
                OnRevert();
                Applied = false;
            }
        }

        public void Commit()
        {
            if (!Commited)
            {
                Commited = true;
                OnCommit();
            }
        }

        public bool ApplyAndCommit()
        {
            if (Apply())
            {
                Commit();
                return true;
            }
            return false;
        }

        public void RevertAndCommit()
        {
            Revert();
            Commit();
        }
    }
}
