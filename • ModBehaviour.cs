using System;
using System.IO;
using UnityEngine;

namespace Unlimited_range
{
    public class ModBehaviour : Duckov.Modding.ModBehaviour
    {
        public float BulletDistance = 99999f;
        private string configPath = string.Empty;

        private void Awake()
        {
            Debug.Log("无限射程模组已加载");

            configPath = Path.Combine(Application.dataPath, "Mods/Unlimited_range/config.txt");
            LoadConfig();

            Debug.Log($"射程已设置为: {BulletDistance}");

            // 每0.5秒强制修改一次当前武器射程
            InvokeRepeating("ForceModifyRange", 0.5f, 0.5f);
        }

        private void ForceModifyRange()
        {
            try
            {
                // 获取当前武器
                var gun = LevelManager.Instance?.MainCharacter?.GetGun();

                if (gun != null)
                {
                    // 直接使用ItemAgent_Gun的Variables修改BulletDistance
                    gun.Variables.GetEntry("BulletDistance").SetFloat(BulletDistance);
                    Debug.Log("射程修改成功");
                }
            }
            catch (Exception e)
            {
                // 输出错误信息
                Debug.LogError($"修改射程时出错: {e.Message}");
            }
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath);
                    foreach (string line in lines)
                    {
                        if (line.Trim().StartsWith("BulletDistance="))
                        {
                            string valueStr = line.Trim().Substring("BulletDistance=".Length).Trim();
                            if (float.TryParse(valueStr, out float distance))
                            {
                                BulletDistance = distance;
                            }
                            break;
                        }
                    }
                }
                else
                {
                    CreateDefaultConfig();
                }
            }
            catch
            {
                // 忽略配置加载错误
            }
        }

        private void CreateDefaultConfig()
        {
            try
            {
                string directory = Path.GetDirectoryName(configPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(configPath, "BulletDistance=99999");
            }
            catch
            {
                // 忽略配置创建错误
            }
        }

        private void OnDestroy()
        {
            CancelInvoke("ForceModifyRange");
        }
    }
}