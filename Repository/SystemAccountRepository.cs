
using BusinessObject.Entity;
using DAO;

namespace Repository
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public List<SystemAccount> GetAllAccount() => SystemAccountDAO.getInstance().GetAllAccount();
        public SystemAccount? GetAccountByEmailAndPassword(string email, string password) => SystemAccountDAO.getInstance().GetAccountByEmailAndPassword(email, password);
        public SystemAccount GetAccountById(short accountID) => SystemAccountDAO.getInstance().GetAccountById(accountID);
        public void UpdateAccount(SystemAccount account) => SystemAccountDAO.getInstance().UpdateAccount(account);
        public void DeleteAccount(SystemAccount account) => SystemAccountDAO.getInstance().DeleteAccount(account);
        public void AddAccount(SystemAccount account) => SystemAccountDAO.getInstance().AddAccount(account);
        public bool IsAdmin(SystemAccount account) => SystemAccountDAO.getInstance().IsAdmin(account);
        public SystemAccount VerifyAccount(SystemAccount account) => SystemAccountDAO.getInstance().VerifyAccount(account);
    }
}

