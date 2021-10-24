using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine
{
    [Order(2)]
    public class TIntersectionFactory : IStaticEntityFactory<Track>
    {
        private readonly ITerrainMap _terrainMap;
        private readonly ILayout _layout;

        public TIntersectionFactory(ITerrainMap terrainMap, ILayout layout)
        {
            _terrainMap = terrainMap;
            _layout = layout;
        }

        public IEnumerable<Track> GetPossibleReplacements(int column, int row, Track track)
        {
            if (_terrainMap.Get(column, row).IsWater)
            {
                yield break;
            }

            var neighbours = track.GetAllNeighbors();
            if (neighbours.Count < 3)
            {
                yield break;
            }

            if (AreAllPresent(neighbours.Up, neighbours.Left, neighbours.Right))
            {
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftUp_RightUp, Style = TIntersectionStyle.CornerAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftUp_RightUp, Style = TIntersectionStyle.CornerAndSecondary };
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftUp_RightUp, Style = TIntersectionStyle.StraightAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftUp_RightUp, Style = TIntersectionStyle.StraightAndSecondary };
            }
            if (AreAllPresent(neighbours.Up, neighbours.Left, neighbours.Down))
            {
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftDown_LeftUp, Style = TIntersectionStyle.CornerAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftDown_LeftUp, Style = TIntersectionStyle.CornerAndSecondary };
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftDown_LeftUp, Style = TIntersectionStyle.StraightAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.LeftDown_LeftUp, Style = TIntersectionStyle.StraightAndSecondary };
            }
            if (AreAllPresent(neighbours.Up, neighbours.Right, neighbours.Down))
            {
                yield return new TIntersection() { Direction = TIntersectionDirection.RightUp_RightDown, Style = TIntersectionStyle.CornerAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.RightUp_RightDown, Style = TIntersectionStyle.CornerAndSecondary };
                yield return new TIntersection() { Direction = TIntersectionDirection.RightUp_RightDown, Style = TIntersectionStyle.StraightAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.RightUp_RightDown, Style = TIntersectionStyle.StraightAndSecondary };
            }
            if (AreAllPresent(neighbours.Down, neighbours.Left, neighbours.Right))
            {
                yield return new TIntersection() { Direction = TIntersectionDirection.RightDown_LeftDown, Style = TIntersectionStyle.CornerAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.RightDown_LeftDown, Style = TIntersectionStyle.CornerAndSecondary };
                yield return new TIntersection() { Direction = TIntersectionDirection.RightDown_LeftDown, Style = TIntersectionStyle.StraightAndPrimary };
                yield return new TIntersection() { Direction = TIntersectionDirection.RightDown_LeftDown, Style = TIntersectionStyle.StraightAndSecondary };
            }
        }

        private static bool AreAllPresent(Track? track1, Track? track2, Track? track3)
            => track1 is not null
            && track2 is not null
            && track3 is not null;

        public bool TryCreateEntity(int column, int row, bool isPartOfDrag, int fromColumn, int fromRow, [NotNullWhen(true)] out Track? entity)
        {
            if (_terrainMap.Get(column, row).IsWater)
            {
                entity = null;
                return false;
            }

            var neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, column, row, emptyIsConsideredConnected: true, ignoreCurrent: isPartOfDrag);

            entity = null;
            if (neighbours.Count == 3)
            {
                if (AreAllPresent(neighbours.Down, neighbours.Left, neighbours.Right))
                {
                    entity = new TIntersection() { Direction = TIntersectionDirection.RightDown_LeftDown };
                }
                else if (AreAllPresent(neighbours.Up, neighbours.Left, neighbours.Right))
                {
                    entity = new TIntersection() { Direction = TIntersectionDirection.LeftUp_RightUp };
                }
                else if (AreAllPresent(neighbours.Up, neighbours.Right, neighbours.Down))
                {
                    entity = new TIntersection() { Direction = TIntersectionDirection.RightUp_RightDown };
                }
                else if (AreAllPresent(neighbours.Up, neighbours.Left, neighbours.Down))
                {
                    entity = new TIntersection() { Direction = TIntersectionDirection.LeftDown_LeftUp };
                }
            }
            // if where we have come from is unhappy, we can fix it, and create a t-intersection
            else if (neighbours.Count == 2 &&
                _layout.TryGet(fromColumn, fromRow, out Track? track) &&
                !track.Happy)
            {
                if (neighbours.Down is not null && neighbours.Up is not null)
                {
                    if (track is SingleTrack singleTrack)
                    {
                        singleTrack.Direction = SingleTrackDirection.Horizontal;
                    }
                    entity = new TIntersection()
                    {
                        Direction = fromColumn < column ? TIntersectionDirection.LeftDown_LeftUp : TIntersectionDirection.RightUp_RightDown
                    };
                }
                else if (neighbours.Left is not null && neighbours.Right is not null)
                {
                    if (track is SingleTrack singleTrack)
                    {
                        singleTrack.Direction = SingleTrackDirection.Vertical;
                    }
                    entity = new TIntersection()
                    {
                        Direction = fromRow < row ? TIntersectionDirection.LeftUp_RightUp : TIntersectionDirection.RightDown_LeftDown
                    };
                }
            }

            return entity is not null;
        }
    }
}
