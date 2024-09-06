using Robot.Common;

namespace YushkevychAndriiRobotChallange;

public class OccupyStationCommand : ICommand
{
    public EnergyStation TargetStation { get; set; }
    public Position GoToPosition { get; set; }
    public RobotCommand Execute()
    {
        return new MoveCommand(){NewPosition = GoToPosition};
    }
}