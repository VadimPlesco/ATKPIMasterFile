using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATKPIMasterFile.BusinessLogic.ViewModels.User
{
    public class UserViewModel
    {
        public long UserId { get; set; }
        public string AvatarUrl { get; set; }
        public int AvatarWidth { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public bool IsModel { get; set; }

        public int Age { get; set; }
        public string City { get; set; }
        public string TextStatus { get; set; }
        public bool IsMy { get; set; }
        public bool IsInMaskMode { get; set; }
        public string MaskUrl { get; set; }
        public string ImageUrlNormal { get; set; }
        public int ImagetHeight { get; set; }
        public bool ImageIsNull { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAskAvatar { get; set; }

        //public int GetHeight(int width)
        //{
        //    return width * Height / Width;
        //}
    }
}
