using GhostChaser.Models;
using GhostChaser.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GhostChaser.ViewModels
{
    /// <summary>
    /// Main ViewModel for the GhostChaser application
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private readonly AuditLogger _auditLogger;
        private GhostType _selectedGhostType;
        private string _ghostName = string.Empty;
        private string _targetSystem = string.Empty;
        private string _username = string.Empty;
        private SecureString? _password;
        private string _domain = string.Empty;
        private bool _useCurrentCredentials = true;
        private bool _isDeploying;
        private string _statusMessage = "Ready";
        private bool _isStatusError;

        // Ghost Account specific
        private string _accountUsername = string.Empty;
        private string _accountDomain = string.Empty;
        private bool _isLocalAccount = true;
        private string _organizationalUnit = string.Empty;

        // Ghost File specific
        private string _filePath = string.Empty;
        private string _fileExtension = ".txt";
        private bool _enableFileAuditing = true;

        // Ghost Share specific
        private string _shareName = string.Empty;
        private string _sharePath = string.Empty;
        private string _shareDescription = string.Empty;
        private bool _enableShareAuditing = true;

        // Ghost SPN specific
        private string _spnServiceClass = "MSSQLSvc";
        private string _spnServiceHost = string.Empty;
        private string _spnServiceAccount = string.Empty;
        private bool _spnCreateServiceAccount = true;
        private bool _enableKerberosAuditing = true;

        public ObservableCollection<Ghost> DeployedGhosts { get; }
        public ObservableCollection<string> SPNServiceClasses { get; }
        public ObservableCollection<string> FileExtensions { get; }

        public ICommand DeployGhostCommand { get; }
        public ICommand RemoveGhostCommand { get; }
        public ICommand ClearStatusCommand { get; }

        public MainViewModel()
        {
            _auditLogger = new AuditLogger();
            DeployedGhosts = new ObservableCollection<Ghost>();
            FileExtensions = new ObservableCollection<string>
            {
                ".txt", ".docx", ".xlsx", ".pdf", ".config", ".xml", ".json", ".sql", ".ps1", ".bat"
            };
            SPNServiceClasses = new ObservableCollection<string>(GhostSPNService.CommonServiceClasses);

            DeployGhostCommand = new RelayCommand(async _ => await DeployGhostAsync(), _ => !IsDeploying);
            RemoveGhostCommand = new RelayCommand(async param => await RemoveGhostAsync(param as Ghost), param => param is Ghost && !IsDeploying);
            ClearStatusCommand = new RelayCommand(_ => ClearStatus());

            _selectedGhostType = GhostType.Account;
            TargetSystem = Environment.MachineName;
        }

        #region Properties

        public GhostType SelectedGhostType
        {
            get => _selectedGhostType;
            set
            {
                if (SetProperty(ref _selectedGhostType, value))
                {
                    OnPropertyChanged(nameof(IsAccountSelected));
                    OnPropertyChanged(nameof(IsFileSelected));
                    OnPropertyChanged(nameof(IsShareSelected));
                    OnPropertyChanged(nameof(IsSPNSelected));
                }
            }
        }

        public bool IsAccountSelected => SelectedGhostType == GhostType.Account;
        public bool IsFileSelected => SelectedGhostType == GhostType.File;
        public bool IsShareSelected => SelectedGhostType == GhostType.Share;
        public bool IsSPNSelected => SelectedGhostType == GhostType.SPN;

        public string GhostName
        {
            get => _ghostName;
            set => SetProperty(ref _ghostName, value);
        }

        public string TargetSystem
        {
            get => _targetSystem;
            set => SetProperty(ref _targetSystem, value);
        }

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public SecureString? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string Domain
        {
            get => _domain;
            set => SetProperty(ref _domain, value);
        }

        public bool UseCurrentCredentials
        {
            get => _useCurrentCredentials;
            set
            {
                if (SetProperty(ref _useCurrentCredentials, value))
                {
                    OnPropertyChanged(nameof(CredentialsEnabled));
                }
            }
        }

        public bool CredentialsEnabled => !UseCurrentCredentials;

        public bool IsDeploying
        {
            get => _isDeploying;
            set => SetProperty(ref _isDeploying, value);
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public bool IsStatusError
        {
            get => _isStatusError;
            set => SetProperty(ref _isStatusError, value);
        }

        // Ghost Account Properties
        public string AccountUsername
        {
            get => _accountUsername;
            set => SetProperty(ref _accountUsername, value);
        }

        public string AccountDomain
        {
            get => _accountDomain;
            set => SetProperty(ref _accountDomain, value);
        }

        public bool IsLocalAccount
        {
            get => _isLocalAccount;
            set
            {
                if (SetProperty(ref _isLocalAccount, value))
                {
                    OnPropertyChanged(nameof(IsDomainAccount));
                }
            }
        }

        public bool IsDomainAccount => !IsLocalAccount;

        public string OrganizationalUnit
        {
            get => _organizationalUnit;
            set => SetProperty(ref _organizationalUnit, value);
        }

        // Ghost File Properties
        public string FilePath
        {
            get => _filePath;
            set => SetProperty(ref _filePath, value);
        }

        public string FileExtension
        {
            get => _fileExtension;
            set => SetProperty(ref _fileExtension, value);
        }

        public bool EnableFileAuditing
        {
            get => _enableFileAuditing;
            set => SetProperty(ref _enableFileAuditing, value);
        }

        // Ghost Share Properties
        public string ShareName
        {
            get => _shareName;
            set => SetProperty(ref _shareName, value);
        }

        public string SharePath
        {
            get => _sharePath;
            set => SetProperty(ref _sharePath, value);
        }

        public string ShareDescription
        {
            get => _shareDescription;
            set => SetProperty(ref _shareDescription, value);
        }

        public bool EnableShareAuditing
        {
            get => _enableShareAuditing;
            set => SetProperty(ref _enableShareAuditing, value);
        }

        // Ghost SPN Properties
        public string SPNServiceClass
        {
            get => _spnServiceClass;
            set => SetProperty(ref _spnServiceClass, value);
        }

        public string SPNServiceHost
        {
            get => _spnServiceHost;
            set => SetProperty(ref _spnServiceHost, value);
        }

        public string SPNServiceAccount
        {
            get => _spnServiceAccount;
            set => SetProperty(ref _spnServiceAccount, value);
        }

        public bool SPNCreateServiceAccount
        {
            get => _spnCreateServiceAccount;
            set => SetProperty(ref _spnCreateServiceAccount, value);
        }

        public bool EnableKerberosAuditing
        {
            get => _enableKerberosAuditing;
            set => SetProperty(ref _enableKerberosAuditing, value);
        }

        #endregion

        private async Task DeployGhostAsync()
        {
            try
            {
                IsDeploying = true;
                IsStatusError = false;
                StatusMessage = "Deploying Ghost...";

                // Validate inputs
                if (string.IsNullOrWhiteSpace(GhostName))
                {
                    SetError("Please provide a Ghost name.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(TargetSystem))
                {
                    SetError("Please specify a target system.");
                    return;
                }

                // Create credentials
                DeploymentCredentials credentials = new DeploymentCredentials
                {
                    UseCurrentCredentials = UseCurrentCredentials,
                    Username = Username,
                    Password = Password?.Copy(),
                    Domain = Domain
                };

                // Create the appropriate Ghost type
                Ghost? ghost = SelectedGhostType switch
                {
                    GhostType.Account => CreateGhostAccount(),
                    GhostType.File => CreateGhostFile(),
                    GhostType.Share => CreateGhostShare(),
                    GhostType.SPN => CreateGhostSPN(),
                    _ => null
                };

                if (ghost == null)
                {
                    SetError("Failed to create Ghost configuration.");
                    return;
                }

                ghost.TargetSystem = TargetSystem;
                ghost.CreatedBy = Environment.UserName;

                // Deploy the Ghost
                IGhostDeploymentService? service = GetDeploymentService(ghost.Type);
                if (service == null)
                {
                    SetError("Deployment service not available.");
                    return;
                }

                DeploymentResult result = await service.DeployAsync(ghost, credentials);

                if (result.Success && result.DeployedGhost != null)
                {
                    DeployedGhosts.Add(result.DeployedGhost);
                    _auditLogger.LogGhostCreation(result.DeployedGhost, Environment.UserName);
                    _auditLogger.LogDeploymentSuccess(result, Environment.UserName);

                    IsStatusError = false;
                    StatusMessage = result.Message;

                    // Clear form
                    ClearForm();
                }
                else
                {
                    _auditLogger.LogDeploymentFailure(result, Environment.UserName);
                    SetError(result.Message);
                }

                credentials.Dispose();
            }
            catch (Exception ex)
            {
                _auditLogger.LogError("DeployGhost", ex, Environment.UserName);
                SetError($"Error deploying Ghost: {ex.Message}");
            }
            finally
            {
                IsDeploying = false;
            }
        }

        private async Task RemoveGhostAsync(Ghost? ghost)
        {
            if (ghost == null) return;

            try
            {
                IsDeploying = true;
                IsStatusError = false;
                StatusMessage = $"Removing Ghost '{ghost.Name}'...";

                var result = MessageBox.Show(
                    $"Are you sure you want to remove Ghost '{ghost.Name}' from {ghost.TargetSystem}?",
                    "Confirm Removal",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    StatusMessage = "Removal cancelled.";
                    return;
                }

                DeploymentCredentials credentials = new DeploymentCredentials
                {
                    UseCurrentCredentials = UseCurrentCredentials,
                    Username = Username,
                    Password = Password?.Copy(),
                    Domain = Domain
                };

                IGhostDeploymentService? service = GetDeploymentService(ghost.Type);
                if (service == null)
                {
                    SetError("Deployment service not available.");
                    return;
                }

                DeploymentResult removeResult = await service.RemoveAsync(ghost, credentials);

                if (removeResult.Success)
                {
                    DeployedGhosts.Remove(ghost);
                    _auditLogger.LogGhostRemoval(ghost, Environment.UserName);

                    IsStatusError = false;
                    StatusMessage = removeResult.Message;
                }
                else
                {
                    SetError(removeResult.Message);
                }

                credentials.Dispose();
            }
            catch (Exception ex)
            {
                _auditLogger.LogError("RemoveGhost", ex, Environment.UserName);
                SetError($"Error removing Ghost: {ex.Message}");
            }
            finally
            {
                IsDeploying = false;
            }
        }

        private GhostAccount? CreateGhostAccount()
        {
            if (string.IsNullOrWhiteSpace(AccountUsername))
            {
                SetError("Please provide an account username.");
                return null;
            }

            return new GhostAccount
            {
                Name = GhostName,
                Username = AccountUsername,
                Domain = AccountDomain,
                IsLocalAccount = IsLocalAccount,
                OrganizationalUnit = string.IsNullOrWhiteSpace(OrganizationalUnit) ? null : OrganizationalUnit,
                Description = $"Ghost canary account - {GhostName}"
            };
        }

        private GhostFile? CreateGhostFile()
        {
            return new GhostFile
            {
                Name = GhostName,
                FilePath = FilePath,
                FileExtension = FileExtension,
                HasAuditingEnabled = EnableFileAuditing,
                Description = $"Ghost canary file - {GhostName}"
            };
        }

        private GhostShare? CreateGhostShare()
        {
            if (string.IsNullOrWhiteSpace(ShareName))
            {
                SetError("Please provide a share name.");
                return null;
            }

            return new GhostShare
            {
                Name = GhostName,
                ShareName = ShareName,
                SharePath = SharePath,
                ShareDescription = string.IsNullOrWhiteSpace(ShareDescription) ?
                    $"Ghost canary share - {GhostName}" : ShareDescription,
                HasAuditingEnabled = EnableShareAuditing,
                Description = $"Ghost canary share - {GhostName}"
            };
        }

        private GhostSPN? CreateGhostSPN()
        {
            if (string.IsNullOrWhiteSpace(SPNServiceClass))
            {
                SetError("Please select a service class (e.g., MSSQLSvc, HTTP).");
                return null;
            }

            if (string.IsNullOrWhiteSpace(SPNServiceHost))
            {
                SetError("Please provide a service host (e.g., sqlserver.domain.com or sqlserver.domain.com:1433).");
                return null;
            }

            if (string.IsNullOrWhiteSpace(SPNServiceAccount))
            {
                SetError("Please provide a service account name for the SPN.");
                return null;
            }

            string spn = $"{SPNServiceClass}/{SPNServiceHost}";

            return new GhostSPN
            {
                Name = GhostName,
                ServiceClass = SPNServiceClass,
                ServiceHost = SPNServiceHost,
                ServicePrincipalName = spn,
                ServiceAccount = SPNServiceAccount,
                Domain = AccountDomain,
                CreateServiceAccount = SPNCreateServiceAccount,
                HasKerberosAuditingEnabled = EnableKerberosAuditing,
                Description = $"Ghost honey SPN for Kerberoasting detection - {GhostName}"
            };
        }

        private IGhostDeploymentService? GetDeploymentService(GhostType type)
        {
            return type switch
            {
                GhostType.Account => new GhostAccountService(),
                GhostType.File => new GhostFileService(),
                GhostType.Share => new GhostShareService(),
                GhostType.SPN => new GhostSPNService(),
                _ => null
            };
        }

        private void SetError(string message)
        {
            IsStatusError = true;
            StatusMessage = message;
        }

        private void ClearStatus()
        {
            StatusMessage = "Ready";
            IsStatusError = false;
        }

        private void ClearForm()
        {
            GhostName = string.Empty;
            AccountUsername = string.Empty;
            FilePath = string.Empty;
            ShareName = string.Empty;
            SharePath = string.Empty;
            ShareDescription = string.Empty;
            SPNServiceHost = string.Empty;
            SPNServiceAccount = string.Empty;
        }
    }
}
