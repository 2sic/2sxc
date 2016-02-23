using ToSic.SexyContent.Environment.Interfaces;

namespace ToSic.SexyContent.Environment.Base
{
    public class Permissions: IPermissions
    {
        /// <summary>
        /// User May Edit Content
        /// </summary>
        public bool UserMayEditContent => false;


        // draft / conceptual properties, not in use yet, don't continue here without daniel
        public bool UserMayViewContent => false;
        public bool UserMayEditData => false;// this would be if the user may edit other data in the app

        public bool UserMayEditConfiguration => false;

        public bool UserMayRemoveContent => false;
        public bool UserMayDeleteData => false;


        public bool UserMayEditMetadata;

        public bool UserMayPublish;
        public bool UserMayPublishContent;
        public bool UserMayPublishData;
        public bool UserMayDesign;

        public bool UserMayDevelop; // c# etc.

        public bool UserMayTranslate;


    }
}