﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace _2048Game
{
    [Serializable]
    public class ScoreBase
    {
        private List<UserInfo> _ScoreBase = new List<UserInfo>();
        private static string _Path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/2048";
        private static string _FileName = _Path + "/scores.bin";

        public List<UserInfo> Scores { get { return _ScoreBase; } set { _ScoreBase = value; } }

        public UserInfo this[int index]
        {
            get
            {
                return _ScoreBase[index];
            }
            set
            {
                _ScoreBase[index] = value;
            }
        }

        public int Find(UserInfo userInfo)
        {
            return _ScoreBase.IndexOf(userInfo);
        }

        public void AddScore(UserInfo userInfo)
        {
            _ScoreBase.Add(userInfo);
            SortScores();
        }

        public bool IsHasScores()
        {
            if (_ScoreBase.Count > 0)
                return true;

            return false;
        }

        private void SortScores()
        {
            UserInfo temp;
            for (int i = 0; i < _ScoreBase.Count; i++)
            {
                for (int j = 0; j < _ScoreBase.Count; j++)
                {
                    if (_ScoreBase[j].Score < _ScoreBase[i].Score)
                    {
                        temp = _ScoreBase[j];
                        _ScoreBase[j] = _ScoreBase[i];
                        _ScoreBase[i] = temp;
                    }
                }
            }
        }

        public int GetBestScore()
        {
            int output = 0;

            for (int i = 0; i < _ScoreBase.Count; i++)
            {
                if (output < _ScoreBase[i].Score)
                    output = _ScoreBase[i].Score;
            }

            return output;
        }

        public static ScoreBase ScoresLoad()
        {
            if (!Directory.Exists(_Path))
                Directory.CreateDirectory(_Path);

            FileStream fileStream;
            try
            {
                fileStream = new FileStream(_FileName, FileMode.Open);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            ScoreBase output;
            try
            {
                output = (ScoreBase)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            }
            catch (Exception e)
            {
                fileStream.Close();
                throw new Exception(e.ToString());
            }

            output.SortScores();

            return output;
        }

        public static void ScoresSave(ScoreBase scoreBase)
        {
            FileStream file;
            try
            {
                file = new FileStream(_FileName, FileMode.Create);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            try
            {
                binaryFormatter.Serialize(file, scoreBase);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }

            file.Close();
        }
    }

    [Serializable]
    public class UserInfo
    {
        private string _UserName;
        private int _Score;

        public string UserName { get { return _UserName; } set { _UserName = value; } }

        public int Score { get { return _Score; } set { _Score = value; } }

        public UserInfo(string username, int score)
        {
            _UserName = username;
            _Score = score;
        }
    }
}
