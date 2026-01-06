using System.Collections.Generic;
using UnityEngine;

namespace Turnbase
{
    [CreateAssetMenu(fileName = "New Skill List", menuName = "Skills/Skill List")]
    public class SkillList : ScriptableObject
    {
        public string listName = "New Skill List";

        // Lưu đường dẫn đến folder chứa Skill
        public string folderPath = "Assets/Skills";

        // Danh sách này sẽ được tự động cập nhật bởi Editor Window
        public List<Skill> skills = new List<Skill>();
    }
}