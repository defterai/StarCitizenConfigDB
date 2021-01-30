using System;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public abstract class Transaction : ITransaction
    {
        public Exception? LastApplyException { get; protected set; }
#if DEBUG
        public Exception? LastRevertException { get; protected set; }
        public Exception? LastCommitException { get; protected set; }
#endif
        public bool Applied { get; private set; }
        public bool Commited { get; private set; }
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
                    CheckRevertException();
                }
                Commited = true;
                OnCommit();
                CheckCommitException();
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
                CheckRevertException();
            }
        }

        public void Commit()
        {
            if (!Commited)
            {
                Commited = true;
                OnCommit();
                CheckCommitException();
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

        private void CheckRevertException()
        {
#if DEBUG
            if (LastRevertException != null)
            {
                throw new Exception("Failed revert transaction: ", LastRevertException);
            }
#endif
        }

        private void CheckCommitException()
        {
#if DEBUG
            if (LastCommitException != null)
            {
                throw new Exception("Failed commit transaction: ", LastCommitException);
            }
#endif
        }
    }
}
