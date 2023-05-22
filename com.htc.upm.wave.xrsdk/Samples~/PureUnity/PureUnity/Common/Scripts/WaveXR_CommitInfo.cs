using System;
using UnityEngine;

namespace Wave.Generic
{
    [Serializable]
    public class CommitInfo
    {
        public static CommitInfo LoadFromResource()
        {
            TextAsset commitInfoAsset = (TextAsset)Resources.Load("commit_info");
            if (commitInfoAsset == null)
                return null;
            return JsonUtility.FromJson<CommitInfo>(commitInfoAsset.text);
        }

        public string commit;
        public string abbreviated_commit;
        public string refs;
    }
}
