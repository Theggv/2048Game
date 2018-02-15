using System;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace _2048Game
{
    [Serializable]
    public class ScoreBase
    {
        private List<UserInfo> _ScoreBase = new List<UserInfo>();
        private const string _FileName = "scores.bin";

        public List<UserInfo> Scores
        {
            get { return _ScoreBase; }
            set { _ScoreBase = value; }
        }

        public void AddScore(UserInfo userInfo)
        {
            _ScoreBase.Add(userInfo);
        }

        public static ScoreBase ScoresLoad()
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(_FileName, FileMode.Open);
            }
            catch
            {
                MessageBox.Show("Файл с результами не найден");
                throw new Exception();
            }
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            ScoreBase output;
            try
            {
                output = (ScoreBase)binaryFormatter.Deserialize(fileStream);
                fileStream.Close();
            }
            catch
            {
                fileStream.Close();
                MessageBox.Show("Файл поврежден.");
                throw new Exception();
            }

            return output;
        }

        public void SaveScores()
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(_FileName, FileMode.CreateNew);
            }
            catch
            {
                MessageBox.Show("Не удалось сохранить результаты.");
                throw new Exception();
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(fileStream, this);

            fileStream.Close();

        }
    }

    [Serializable]
    public class UserInfo
    {
        private string _UserName;
        private int _Score;

        public string UserName
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public int Score
        {
            get { return _Score; }
            set { _Score = value; }
        }

        public UserInfo(string username, int score)
        {
            _UserName = username;
            _Score = score;
        }
    }
}
