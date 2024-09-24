using Robot.Common;

namespace YushkevychAndriiRobotChallange.Commands;

public class OccupyStationCommand : Command
{
    public EnergyStation TargetStation { get; set; }
    public Position GoToPosition { get; set; }
    public override void Process()
    {
        var command = Algorithm.MyRobots?[Algorithm.RobotToMoveIndex] as OccupyStationCommand;

        if (command?.TargetStation.Position == Algorithm.Robots?[Algorithm.RobotToMoveIndex].Position)
        {
            Algorithm.TransitionTo(new CollectCommand{TargetStation = command?.TargetStation});
        }
        else if(command is not null)
        {
            Algorithm.TransitionTo(new OccupyStationCommand
            {
                TargetStation = command.TargetStation,
                GoToPosition = Movement.FindDestination(
                    command.TargetStation.Position,
                    Algorithm.Robots?[Algorithm.RobotToMoveIndex], 
                    Movement.FindDistance(command.TargetStation.Position, Algorithm.Robots?[Algorithm.RobotToMoveIndex]))
            });
        }
    }

    public override RobotCommand Execute()
    {
        return new MoveCommand{NewPosition = GoToPosition};
    }
}