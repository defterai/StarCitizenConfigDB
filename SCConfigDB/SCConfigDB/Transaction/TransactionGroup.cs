using System;
using System.Collections.Generic;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public sealed class TransactionGroup : Transaction
    {
        private readonly List<Transaction> _transactions;
        public int Count => _transactions.Count;
        public Transaction? FailedTransaction { get; private set; }

        public TransactionGroup()
        {
            _transactions = new List<Transaction>();
        }

        public TransactionGroup(int capacity)
        {
            _transactions = new List<Transaction>(capacity);
        }

        private TransactionGroup(TransactionGroup group)
        {
            _transactions = new List<Transaction>(group._transactions);
            Applied = group.Applied;
            Commited = group.Commited;
            group._transactions.Clear();
            group.Applied = true;
            group.Commited = true;
        }

        public new void Dispose()
        {
            base.Dispose();
            if (_transactions.Count != 0)
            {
                DisposeTransactions(_transactions);
                _transactions.Clear();
            }
        }

        public void Add(Transaction transaction)
        {
            if (Commited || Applied != transaction.Applied)
                throw new InvalidOperationException("Can't add transaction to group");
            _transactions.Add(transaction);
        }

        public bool Remove(Transaction transaction, bool dispose = true)
        {
            if (Commited || Applied != transaction.Applied)
                throw new InvalidOperationException("Can't remove transaction from group");
            bool removed = _transactions.Remove(transaction);
            if (removed && dispose)
            {
                transaction.Dispose();
            }
            return removed;
        }

        public void Clear(bool dispose = true)
        {
            if (_transactions.Count != 0)
            {
                if (Commited || Applied)
                    throw new InvalidOperationException("Can't clear applied transation group");
                if (dispose)
                {
                    DisposeTransactions(_transactions);
                }
                _transactions.Clear();
            }
        }

        public TransactionGroup? ApplyAndCommitPartially()
        {
            if (Commited)
                throw new InvalidOperationException("Can't apply after commit transation");
            if (!Applied)
            {
                int failedCount = 0;
                foreach (var transaction in _transactions)
                {
                    if (!transaction.Apply())
                    {
                        failedCount++;
                    }
                }
                if (failedCount != 0)
                {
                    if (failedCount == _transactions.Count)
                    {
                        return new TransactionGroup(this);
                    }
                    var failedGroup = new TransactionGroup(failedCount);
                    foreach (var transaction in _transactions)
                    {
                        if (!transaction.Applied)
                        {
                            failedGroup.Add(transaction);
                            if (failedGroup.Count >= failedCount)
                            {
                                break;
                            }
                        }
                    }
                    _transactions.RemoveAll(t => !t.Applied);
                    Applied = true;
                    Commit();
                    return failedGroup;
                }
                Applied = true;
            }
            Commit();
            return null;
        }

        protected override bool OnApply()
        {
            int appliedCount = ApplyTransactions(_transactions);
            if (appliedCount < _transactions.Count)
            {
                FailedTransaction = _transactions[appliedCount];
                RevertTransactions(_transactions, appliedCount);
                return false;
            }
            return true;
        }

        protected override void OnRevert() => RevertTransactions(_transactions, _transactions.Count);

        protected override void OnCommit()
        {
            if (_transactions.Count != 0)
            {
                CommitTransactions(_transactions);
                _transactions.Clear();
            }
        }

        private static void DisposeTransactions(IList<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                transaction.Dispose();
            }
        }

        private static int ApplyTransactions(IList<Transaction> transactions)
        {
            for (int i = 0; i < transactions.Count; i++)
            {
                if (!transactions[i].Apply())
                {
                    return i;
                }
            }
            return transactions.Count;
        }

        private static void RevertTransactions(IList<Transaction> transactions, int revertCount)
        {
            for (int i = 0; i < revertCount; i++)
            {
                transactions[i].Revert();
            }
        }

        private static void CommitTransactions(IList<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                transaction.Commit();
            }
        }
    }
}
