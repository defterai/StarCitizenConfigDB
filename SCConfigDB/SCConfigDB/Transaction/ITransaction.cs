using System;

namespace Defter.StarCitizen.ConfigDB.Transaction
{
    public interface ITransaction : IDisposable
    {
        bool Applied { get; }
        bool Commited { get; }
        bool Apply();
        void Revert();
        void Commit();
        bool ApplyAndCommit();
        void RevertAndCommit();
    }
}
