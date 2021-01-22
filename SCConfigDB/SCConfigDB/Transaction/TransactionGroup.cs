using System;
using System.Collections.Generic;
using System.Linq;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public sealed class TransactionGroup : Transaction
    {
        private readonly List<Transaction> _transactions;
        public IReadOnlyList<Transaction> Transactions => _transactions;
        public Transaction? LastFailToApplyTransaction { get; private set; }
        public bool ReverseCommitOrder { get; set; }

        public TransactionGroup()
        {
            _transactions = new List<Transaction>();
        }

        public TransactionGroup(int capacity)
        {
            _transactions = new List<Transaction>(capacity);
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
                    throw new InvalidOperationException("Can't remove applied transations from group");
                if (dispose)
                {
                    DisposeTransactions(ReverseCommitOrder ? _transactions.Reverse<Transaction>() : _transactions);
                }
                _transactions.Clear();
            }
        }

        protected override bool OnApply()
        {
            int appliedCount = ApplyTransactions(_transactions);
            if (appliedCount < _transactions.Count)
            {
                LastFailToApplyTransaction = _transactions[appliedCount];
                LastApplyException = LastFailToApplyTransaction.LastApplyException;
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
                CommitTransactions(ReverseCommitOrder ? _transactions.Reverse<Transaction>() : _transactions);
                _transactions.Clear();
            }
        }

        private static void DisposeTransactions(IEnumerable<Transaction> transactions)
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
            for (int i = revertCount - 1; i >= 0; i--)
            {
                transactions[i].Revert();
            }
        }

        private static void CommitTransactions(IEnumerable<Transaction> transactions)
        {
            foreach (var transaction in transactions)
            {
                transaction.Commit();
            }
        }
    }
}
