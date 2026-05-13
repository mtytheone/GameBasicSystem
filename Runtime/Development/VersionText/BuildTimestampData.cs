#if DEBUG
using System;
using UnityEngine;

namespace HatzeLaboratory.GameBasicSystem.Runtime.Development.VersionText
{
    [CreateAssetMenu(fileName = "BuildTimestampData", menuName = "LBGame/CreateBuildTimestampData")]
    public class BuildTimestampData : ScriptableObject
    {
        [SerializeField]
        private int _year;

        [SerializeField]
        private int _month;

        [SerializeField]
        private int _day;

        [SerializeField]
        private int _hour;

        [SerializeField]
        private int _minute;

        [SerializeField]
        private int _second;

        [SerializeField]
        private int _millisecond;


        public BuildTimestampData()
        {
            if (_year == 0 && _month == 0 && _day == 0)
            {
                SetBuildTimeStamp(DateTime.Now);
            }
        }

        public void SetBuildTimeStamp(DateTime buildTimestamp)
        {
            _year = buildTimestamp.Year;
            _month = buildTimestamp.Month;
            _day = buildTimestamp.Day;
            _hour = buildTimestamp.Hour;
            _minute = buildTimestamp.Minute;
            _second = buildTimestamp.Second;
            _millisecond = buildTimestamp.Millisecond;
        }

        public DateTime GetBuildTimeStamp()
        {
            return new DateTime(_year, _month, _day, _hour, _minute, _second, _millisecond);
        }
    }
}
#endif