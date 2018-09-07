using System;
using UnityEngine;

namespace KMUtility.Unity
{
    /// <summary>
    /// シングルトン抽象クラス
    /// </summary>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T m_instance;
        public static T Instance
        {
            get
            {
                if (m_instance == null)
                {
                    Type t = typeof(T);
                    m_instance = (T)FindObjectOfType(t);
                    if (m_instance == null)
                        Debug.LogError(t + " をアタッチしているGameObjectはありません");
                }

                return m_instance;
            }
        }

        virtual protected void Awake()
        {
            // シーンが映っても破壊されないようにする
            DontDestroyOnLoad(gameObject);
            // 他のゲームオブジェクトにアタッチされているか調べる
            // アタッチされている場合は破棄する。
            CheckInstance();
        }

        protected bool CheckInstance()
        {
            if (m_instance == null)
            {
                m_instance = this as T;
                return true;
            }
            else if (Instance == this)
            {
                return true;
            }
            Destroy(this);
            return false;
        }
    }
}