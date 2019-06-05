using System;
using System.ComponentModel.DataAnnotations;
namespace ATKPIMasterFile.DataAccess.Model
{
    public enum SexEnum : byte
    {
        any = 0,
        male = 1,
        female = 2
    }

    public enum BdsmRole
    {
        None = 0,
        MaleDominant = 1,
        FemaleDominant = 2,
        MaleSubmissive = 3,
        FemaleSubmissive = 4
    }

    public enum AuthProviderEnum : byte
    {
        native = 0,
        vk = 1,
        facebook = 2,
        twitter = 3,
        google = 4,
        mailru = 5,
        odnoklassniki = 6
    }

    public enum ChatMessageTypeEnum : byte
    {
        textMessage = 1,
        money = 2,
        gift = 3,
        photo = 4,
        owenPhotoComment = 5,
        interestPhotoComment = 6,
        askPhoto = 9,
        askPersonalInfo = 10,
        advertClick = 11,
        askAvatar = 13,
        addPhotoAfterAsk = 14,
        addPersonalInfoAfterAsk = 15,
        addAvatarAfterAsk = 16,
        adminBannedAvatar = 18,
        adminBannedPhoto = 19,
        adminBannedStatus = 20,
        adminBannedComment = 21,
        adminBannedInterest = 22,
        adminMoveInterestItems = 23,
        strangePhotoComment = 25,
        strangeInterestPhotoComment = 26,
        SocialStatusChanged = 27,
        PaidPhotoModeration = 30,
        buyPremiumStatus = 33,
        moderationAlert = 34,
        AdminSexyChat = 35,
        buyMaskMode = 36,
        rebillMaskMode = 37,
        cancelMaskMode = 38,
        deleteMaskMode = 39,
        moderationLastAlert = 40,
        HotOrNotWinner = 41,
        buyClubCard = 42,
        rebillClubCard = 43,
        cancelClubCard = 44,
        deleteClubCard = 45,
        SexyChatRequest = 50,
        SexyChatConfirm = 51,
        SexyChatReject = 52,
        SexyChatEnd = 53,
        PrivateVideoChatStart = 54,
        PrivateVideoChatEnd = 55
    }

    public enum LastEventTypeEnum : byte
    {
        photoReblog = 8,
        subscribe = 12,
        openPrivatePhoto = 17,
        viewProfile = 28,
        InterestPhotoLike = 29,
        PaidPhotoRate = 31,
        PersonalPhotoLike = 32,
    }

    public enum AdminEventType
    {
        ModelIsBroadcasting = 0,
        PrivateChatStart = 1,
        PrivateChatEnd = 2
    }

    [AttributeUsageAttribute(AttributeTargets.Field, Inherited = false)]
    public class ImportantNotificationAttribute : Attribute { }

    public enum NotificationType
    {
        /// <summary>
        /// User disconnected
        /// </summary>
        Disconnect = 0,

        /// <summary>
        /// User connected
        /// </summary>
        Connect = 1,

        /// <summary>
        /// User owner if interest item
        /// </summary>
        Like = 2,

        /// <summary>
        /// user owner and interims of interest item
        /// </summary>
        Reblog = 3,

        /// <summary>
        /// Someone subscribed to interest owner
        /// </summary>
        Subscribe = 4,

        /// <summary>
        /// Someone subscribed to interest owner
        /// </summary>
        SubscribeAll = 40,

        /// <summary>
        /// User added photo broadcast
        /// </summary>
        AddPhoto = 5,

        /// <summary>
        /// user started broadcasting to all
        /// </summary>
        [ImportantNotification]
        BroadcastStarted = 6,

        /// <summary>
        /// All
        /// </summary>
        LikeCount = 7,

        /// <summary>
        /// not used
        /// </summary>
        PrivateLikeCount = 8,

        /// <summary>
        /// vp
        /// </summary>
        ViewProfile = 9,

        [ImportantNotification]
        PhotoUploadSuccess = 10,

        FlashAdverCreateSuccess = 11,

        /// <summary>
        /// not used
        /// </summary>
        Advert = 12,
        [ImportantNotification]
        PhotoReblogSuccess = 13,

        AddText = 14,
        [ImportantNotification]
        FeedbackSentSuccess = 15,

        [ImportantNotification]
        VideoChatStarted = 16,
        [ImportantNotification]
        VideoChatStopped = 17,

        /// <summary>
        /// user finished broadcasting to all
        /// </summary>
        [ImportantNotification]
        BroadcastStopped = 18,

        [ImportantNotification]
        PrivateVideoChatAccepted = 19,
        [ImportantNotification]
        ModelIsBusyInPrivateChat = 20,
        [ImportantNotification]
        ModelIsFreeForPrivateChat = 21
    }

    public enum EventSpecificTypeEnum : byte
    {
        openPrivatePhoto = 1
    }

    public enum FeedTypeEnum
    {
        interestPhoto = 3,
        interestText = 4,
        addOwnPhoto = 9,
        askOwnPhotoToAdd = 10,
        addInterestPhoto = 11,
        askInterestPhotoToAdd = 12,
    }

    public enum InterestItemTypeEnum : byte
    {
        photo = 1,
        text = 2,
        broadcasting = 3,
        chatPhoto = 4
    }

    public enum ResponseCode : byte
    {
        Ok,
        Error,
        Conflict,
        BadParams,
        UserNotFound,
        NotFound,
        UserWithEmailNotFound,
        NoEnoughFunds,
        CertificateIsDisabled,
        AuthenticationHasExpired,
        AuthenticationError,
        ImageFileCouldNotBeOpened,
        UnexpectedImageFormat,
        TextContainsInvalidCharacters,
        EmailAlreadyExists,
        PageAddressAlreadyExists,
        Blocker,
        BlockerBottom,
        RedirectToBody,
        ContactsLimitExceeded,
        MessageLimitPerContactExceeded,
        HideModal,
    }

    public enum DiamondsLogTypeEnum : byte
    {
        Gift = 1,
        cashout = 9,
        refillViaWebMoney = 15,
        refillViaOplataInfo = 16,
        refillViaSmsCoin = 17,
        refillViaVerotelFlexPay = 18,
        buyPremium = 19,
        refillViaVerotelMembership = 20,
        refillViaDengiOnline = 21,
        refillAfterBuyClubCard = 22,

        buyAdvert = 30,
        FirstBroadcastPay = 31,
        FreeBroadcastingPayment = 32,
        PrivateBroadcastingPayment = 33
    }

    public enum GoodsOperationsTypeEnum : byte
    {
        none = 0,
        verotelMembershipAddPremiumStatus = 1,
        verotelMembershipRebillPremiumStatus = 2,
        verotelMembershipModifyPremiumStatus = 3,
        verotelMembershipCancelPremiumStatus = 4,
        verotelMembershipDeletePremiumStatus = 5,
        verotelFlexPayPremiumStatus = 6,
        webMoneyPremiumStatus = 7,
        oplataInfoPremiumStatus = 8,
        verotelMembershipAddMaskMode = 9,
        verotelMembershipRebillMaskMode = 10,
        verotelMembershipModifyMaskMode = 11,
        verotelMembershipCancelMaskMode = 12,
        verotelMembershipDeleteMaskMode = 13,
        verotelFlexPayMaskMode = 14,
        webMoneyMaskMode = 15,
        oplataInfoMaskMode = 16,
        verotelMembershipAddClubCard = 17,
        verotelMembershipRebillClubCard = 18,
        verotelMembershipModifyClubCard = 19,
        verotelMembershipCancelClubCard = 20,
        verotelMembershipDeleteClubCard = 21,
        dengiOnlinePremiumStatus = 22,
        dengiOnlineMaskMode = 23
    }

    public enum GoodsTypeEnum : byte
    {
        coins = 1,
        premiumStatus = 2,
        maskMode = 3,
        clubCard = 4,
        clubCard1 = 5
    }

    public enum PaymentServiceTypeEnum : byte
    {
        verotel = 1,
        webMoney = 2,
        yandex = 3,
        qiwi = 4,
        mailRu = 5
    }


    public enum ModerationStatusEnum : byte
    {
        NotModerate = 0,
        NotModeratePhoto = 1,
        NotModerateText = 2,
        Moderated = 3,
        ApproveErotic = 4,
        // NotApprove = 5,
        // NotApprovePhoto = 6,
        // NotApproveText = 7
        Ban = 9
    }

    public enum Flash : byte
    {
        Free = 0,
        Activ = 1,
        Turbo = 2
    }

    public enum AskEnum : byte
    {
        Avatar = 0,
        PersonalInfo = 1,
        InterestItem = 2
    }

    public enum MailNotificationEnum : byte
    {
        MessageForYou = 0,
        YourPhotoAppreciated = 1,
        YourPhotoReblogged = 2
    }

    public enum SendMailTemplateTypeEnum : byte
    {
        MessageEmail = 0,
        ProfileEmail = 1
    }

    public enum UserActiveStatusEnum : byte
    {
        ok = 0,
        banned = 1,
        missingfields = 2
    }

    public enum LocaleEnum
    {
        [Display(Name = "En")]
        en = 9,
        [Display(Name = "Ru")]
        ru = 25,
    }

    [Flags]
    public enum FeedViewSize : byte
    {
        Small = 0,
        FeedBig = 1,
        InterestBig = 1 << 1,
        PersonalInterestBig = 1 << 2,
        DatingBig = 1 << 3,
    }

    public enum ModerationResource
    {
        Interests,
        InterestPhotos,
        Users
    }


    public static class EnumEx
    {
        public static T SetFlag<T>(this Enum value, T flag, bool set)
        {
            Type underlyingType = Enum.GetUnderlyingType(value.GetType());

            // note: AsInt mean: math integer vs enum (not the c# int type)
            dynamic valueAsInt = Convert.ChangeType(value, underlyingType);
            dynamic flagAsInt = Convert.ChangeType(flag, underlyingType);
            if (set)
            {
                valueAsInt |= flagAsInt;
            }
            else
            {
                valueAsInt &= ~flagAsInt;
            }

            return (T)valueAsInt;
        }
    }
}