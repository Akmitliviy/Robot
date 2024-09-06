using Robot.Common;

namespace YushkevychAndriiRobotChallange;

public class CollectCommand : ICommand
{
    public RobotCommand Execute()
    {
        return new CollectEnergyCommand();
    }
}