using System;
using System.IO;
using DBFilesViewer.Utils.Extensions;

namespace DBFilesViewer.Graphics.Files.Models.Animations
{
    public class ModelAnimationBlock<T> : ModelAnimationBlock<T, T, NoInterpolate<T>> where T : struct
    {
        public ModelAnimationBlock(M2 file, MD20 header, AnimationBlock<T> data, T defaultValue = default(T)) : base(file, header, data, defaultValue)
        {
        }
    }

    public class ModelAnimationBlock<T, TInterpolator> : ModelAnimationBlock<T, T, TInterpolator> where TInterpolator : IInterpolator<T, T>, new() where T : struct
    {
        public ModelAnimationBlock(M2 file, MD20 header, AnimationBlock<T> data, T defaultValue = default(T)) : base(file, header, data, defaultValue)
        {
        }
    }

    public class ModelAnimationBlock<TSource, TDest, TInterpolator> where TInterpolator : IInterpolator<TSource, TDest>, new() where TSource : struct
    {
        private static readonly TInterpolator Interpolator = new TInterpolator();

        private AnimationBlock<TSource> _fileBlock;
        private TDest _defaultValue;
        private uint? _globalSequence;

        public InterpolationType InterpolationType => _fileBlock.Interpolation;

        private uint[][] _timeStamps;
        private TSource[][] _values;

        public ModelAnimationBlock(M2 file, MD20 header, AnimationBlock<TSource> data, TDest defaultValue = default(TDest))
        {
            if (data.GlobalSequence >= 0 && data.GlobalSequence < header.GlobalLoops.Length)
                _globalSequence = header.GlobalLoops[data.GlobalSequence];

            _defaultValue = defaultValue;
            _fileBlock = data;

            if (InterpolationType > InterpolationType.Linear)
                Console.WriteLine("[DEBUG] Missing data to perform a {0} interpolation!", _fileBlock.Interpolation);

            //! TODO Remove this hack
            Interpolator.SetInterpolationType(InterpolationType.Linear);

            Load(file);
        }

        public TDest GetValue(int timeline, uint time, uint length)
        {
            if (timeline >= _timeStamps.Length || timeline >= _values.Length)
                return _defaultValue;

            var currentTimeline = _timeStamps[timeline];
            var currentTimelineValues = _values[timeline];

            if (currentTimeline.Length == 0 || currentTimelineValues.Length == 0)
                return _defaultValue;

            if (_globalSequence.HasValue && _globalSequence > 0)
                time %= _globalSequence.Value;
            else
            {
                var ltime = currentTimeline[currentTimeline.Length - 1];
                if (ltime != 0)
                    time %= ltime;
            }

            var maxIndex = Math.Min(currentTimeline.Length, currentTimelineValues.Length);
            if (maxIndex == 1)
                return Interpolator.Interpolate(ref currentTimelineValues[0]);

            if (time == currentTimeline[maxIndex - 1])
                return Interpolator.Interpolate(ref currentTimelineValues[maxIndex - 1]);

            if (time >= currentTimeline[maxIndex - 1])
            {
                var timeEnd = currentTimeline[0] + length;
                var timeStart = currentTimeline[maxIndex - 1];
                var fac = (time - timeStart) / (float)(timeEnd - timeStart);
                return Interpolator.Interpolate(fac, ref currentTimelineValues[maxIndex - 1], ref currentTimelineValues[0]);
            }

            return time <= currentTimeline[0] ? Interpolator.Interpolate(ref currentTimelineValues[0]) : InterpolateValue(time, timeline);
        }

        public TDest GetValue(int timelineIndex, uint timeFull)
        {
            if (timelineIndex >= _timeStamps.Length || timelineIndex >= _values.Length)
                return _defaultValue;

            var timeLine = _timeStamps[timelineIndex];
            var values = _values[timelineIndex];

            if (timeLine.Length == 0 || values.Length == 0)
                return _defaultValue;

            if (_globalSequence.HasValue && _globalSequence > 0)
                timeFull %= _globalSequence.Value;
            else
            {
                var ltime = timeLine[timeLine.Length - 1];
                if (ltime != 0)
                    timeFull %= ltime;
            }

            return InterpolateValue(timeFull, timelineIndex);
        }

        private TDest InterpolateValue(uint time, int timeline)
        {
            var currentTimeline = _timeStamps[timeline];
            var currentTimelineValues = _values[timeline];

            var maxIndex = Math.Min(currentTimeline.Length, currentTimelineValues.Length);

            int istart;
            var iend = 0;

            var found = false;

            for (istart = 0; istart < maxIndex - 1; ++istart)
            {
                if (currentTimeline[istart] <= time && currentTimeline[istart + 1] >= time)
                {
                    iend = istart + 1;
                    found = true;
                    break;
                }
            }

            if (!found)
                return Interpolator.Interpolate(ref currentTimelineValues[istart]);

            var timeStart = currentTimeline[istart];
            var timeEnd = currentTimeline[iend];

            if (timeStart > timeEnd)
            {
                var tmp = timeStart;
                timeStart = timeEnd;
                timeEnd = tmp;
            }

            if (timeStart > time)
                timeStart = time;
            if (timeEnd < time)
                timeEnd = time;

            var fac = (time - timeStart) / (float)(timeEnd - timeStart);
            return Interpolator.Interpolate(fac, ref currentTimelineValues[istart], ref currentTimelineValues[iend]);
        }

        private void Load(BinaryReader reader)
        {
            var timelineEntries = ReadArrayOf(reader, ref _fileBlock.Timestamps);
            _timeStamps = new uint[timelineEntries.Length][];
            for (var i = 0; i < timelineEntries.Length; ++i)
                _timeStamps[i] = ReadArrayOf(reader, ref timelineEntries[i]);

            var valuesEntries = ReadArrayOf(reader, ref _fileBlock.Values);
            _values = new TSource[valuesEntries.Length][];
            for (var i = 0; i < valuesEntries.Length; ++i)
                _values[i] = ReadArrayOf(reader, ref valuesEntries[i]);
        }

        private T[] ReadArrayOf<T>(BinaryReader reader, ref M2Array<T> arr) where T : struct
        {
            if (arr.Count == 0)
                return new T[0];

            reader.BaseStream.Position = arr.Offset + 8; //! TODO: Can also be 0 if not chunked file
            return reader.ReadArray<T>(arr.Count);
        }
    }
}
