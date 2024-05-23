using System.Diagnostics.CodeAnalysis;

namespace Trains.NET.Engine;

[Order(2)]
public class TIntersectionFactory(ITerrainMap terrainMap, ILayout layout) : IStaticEntityFactory<Track>
{
    private readonly ITerrainMap _terrainMap = terrainMap;
    private readonly ILayout _layout = layout;

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

    public bool TryCreateEntity(int column, int row, int fromColumn, int fromRow, [NotNullWhen(true)] out Track? entity)
    {
        if (_terrainMap.Get(column, row).IsWater)
        {
            entity = null;
            return false;
        }

        var neighbours = TrackNeighbors.GetConnectedNeighbours(_layout, column, row, emptyIsConsideredConnected: true, ignoreCurrent: fromColumn != 0);
        var allNeighbours = TrackNeighbors.GetAllNeighbours(_layout, column, row);

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
        else if (neighbours.Count == 2 && allNeighbours.Count == 3)
        {
            if (neighbours.Up is not null && neighbours.Down is not null && allNeighbours.Left is SingleTrack { Happy: false } singleTrack1)
            {
                entity = new TIntersection() { Direction = TIntersectionDirection.LeftDown_LeftUp };
                if (singleTrack1.IsConnectedDown() && singleTrack1.GetAllNeighbors().Down is not null)
                {
                    singleTrack1.Direction = SingleTrackDirection.RightDown;
                }
                else if (singleTrack1.IsConnectedUp() && singleTrack1.GetAllNeighbors().Up is not null)
                {
                    singleTrack1.Direction = SingleTrackDirection.RightUp;
                }
                else
                {
                    singleTrack1.Direction = SingleTrackDirection.Horizontal;
                }
            }
            else if (neighbours.Up is not null && neighbours.Down is not null && allNeighbours.Right is SingleTrack { Happy: false } singleTrack2)
            {
                entity = new TIntersection() { Direction = TIntersectionDirection.RightUp_RightDown };
                if (singleTrack2.IsConnectedDown() && singleTrack2.GetAllNeighbors().Down is not null)
                {
                    singleTrack2.Direction = SingleTrackDirection.LeftDown;
                }
                else if (singleTrack2.IsConnectedUp() && singleTrack2.GetAllNeighbors().Up is not null)
                {
                    singleTrack2.Direction = SingleTrackDirection.LeftUp;
                }
                else
                {
                    singleTrack2.Direction = SingleTrackDirection.Horizontal;
                }
            }
            else if (neighbours.Left is not null && neighbours.Right is not null && allNeighbours.Up is SingleTrack { Happy: false } singleTrack3)
            {
                entity = new TIntersection() { Direction = TIntersectionDirection.LeftUp_RightUp };
                if (singleTrack3.IsConnectedLeft() && singleTrack3.GetAllNeighbors().Left is not null)
                {
                    singleTrack3.Direction = SingleTrackDirection.LeftDown;
                }
                else if (singleTrack3.IsConnectedRight() && singleTrack3.GetAllNeighbors().Right is not null)
                {
                    singleTrack3.Direction = SingleTrackDirection.RightDown;
                }
                else
                {
                    singleTrack3.Direction = SingleTrackDirection.Vertical;
                }
            }
            else if (neighbours.Left is not null && neighbours.Right is not null && allNeighbours.Down is SingleTrack { Happy: false } singleTrack4)
            {
                entity = new TIntersection() { Direction = TIntersectionDirection.RightDown_LeftDown };
                if (singleTrack4.IsConnectedLeft() && singleTrack4.GetAllNeighbors().Left is not null)
                {
                    singleTrack4.Direction = SingleTrackDirection.LeftUp;
                }
                else if (singleTrack4.IsConnectedRight() && singleTrack4.GetAllNeighbors().Right is not null)
                {
                    singleTrack4.Direction = SingleTrackDirection.RightUp;
                }
                else
                {
                    singleTrack4.Direction = SingleTrackDirection.Vertical;
                }
            }
        }

        return entity is not null;
    }
}
