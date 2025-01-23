using PaperFy.Shared.Windows.Events;
using PaperFy.Shared.Windows.Services;
using System.Text.Json.Serialization;

namespace PaperFy.Shared.AppManager
{
    public class Settings
    {
        private const string Filename = "Persisted Data";

        private static bool isLoading = false;

        private bool captureLabels = true;

        private bool captureKeyboardEvents;

        private bool captureSpecialKeys = true;

        private string taskID;

        private string token;

        //private UserResponseType? user;

        public static Settings Instance { get; private set; } = new Settings();

        [JsonPropertyName("capture_keyboard_events")]
        public bool CaptureKeyboardEvents
        {
            get
            {
                return captureKeyboardEvents;
            }
            set
            {
                if (captureKeyboardEvents != value)
                {
                    captureKeyboardEvents = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("CaptureKeyboardEvents", CaptureKeyboardEvents));
                    }
                }
            }
        }

        [JsonPropertyName("capture_labels")]
        public bool CaptureLabels
        {
            get
            {
                return captureLabels;
            }
            set
            {
                if (captureLabels != value)
                {
                    captureLabels = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("CaptureLabels", CaptureLabels));
                    }
                }
            }
        }

        [JsonPropertyName("capture_special_keys")]
        public bool CaptureSpecialKeys
        {
            get
            {
                return captureSpecialKeys;
            }
            set
            {
                if (captureSpecialKeys != value)
                {
                    captureSpecialKeys = value;
                    if (!isLoading)
                    {
                        EventAggregator.Instance.Publish(new SettingChangedEvent("CaptureSpecialKeys", CaptureSpecialKeys));                      
                    }
                }
            }
        }

        //[JsonPropertyName("user")]
        //public UserResponseType? User
        //{
        //	get
        //	{
        //		return user;
        //	}
        //	set
        //	{
        //		if (!(user == value))
        //		{
        //			user = ((value?.ID != null) ? value : null);
        //			if (!isLoading)
        //			{
        //				EventAggregator.Instance.Publish(new SettingChangedEvent("User", User));
        //				Save();
        //			}
        //		}
        //	}
        //}

        //[JsonPropertyName("user")]
        //public UserResponseType? User
        //{
        //    get => user ?? new UserResponseType
        //    {
        //        ID = "mock-id",
        //        Email = "mock@email.com",
        //        FirstName = "MockFirst",
        //        LastName = "MockLast",
        //        IsEmailVerified = true,
        //        InstalledDesktopRecorder = false,
        //        ProfilePictureUrl = "",
        //        Organizations = new List<OrganizationResponseType>(),
        //        ActiveOrganization = new OrganizationResponseType
        //        {
        //            ID = "mock-org-id",
        //            Name = "Mock Org",
        //            PermissionLevel = PermissionLevelResponseType.Admin,
        //            SmartPrivacyScreenEnabled = false,
        //            SuperOrganization = new SuperOrganizationResponseType
        //            {
        //                ID = "mock-super-org-id",
        //                Name = "Mock Super Org",
        //                AllowDesktop = true,
        //                ShowBlockWorkspaceModal = false
        //            }
        //        }
        //    };
        //    set
        //    {
        //        if (user != value)
        //        {
        //            user = (value?.ID != null) ? value : User;
        //            if (!isLoading)
        //            {
        //                EventAggregator.Instance.Publish(new SettingChangedEvent("User", User));
        //                Save();
        //            }
        //        }
        //    }
        //}

        protected string SettingsFilePath => Path.Combine(SystemService.Instance.LocalApplicationDataPath, "Persisted Data");

        [JsonConstructor]
        private Settings()
        {
        }





    }
}
