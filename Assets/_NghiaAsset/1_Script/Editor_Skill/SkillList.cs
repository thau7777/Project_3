using System.Collections.Generic;
using UnityEngine;

namespace Turnbase
{
    // Đặt trong thư mục "Assets/Skills/Lists"
    [CreateAssetMenu(fileName = "New Skill List", menuName = "Skills/Skill List")]
    public class SkillList : ScriptableObject
    {
        // Tên của danh sách (ví dụ: "Fighter Skills", "Mage Spells")
        public string listName = "New Skill List";

        // Danh sách các đối tượng Skill mà bạn đã định nghĩa trước đó
        public List<Skill> skills = new List<Skill>();
    }
}