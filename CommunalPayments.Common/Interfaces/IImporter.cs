using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace CommunalPayments.Common.Interfaces
{
    public interface IImporter
    {
        Task<bool> Import(string login, string password, int userId);
        event EventHandler<ProgressChangedEventArgs> ImportProgressChanged;
    }
}
