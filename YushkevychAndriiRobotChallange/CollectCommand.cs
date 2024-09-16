using Robot.Common;

namespace YushkevychAndriiRobotChallange;

public class CollectCommand : ICommand
{
    public EnergyStation TargetStation { get; set; }
    public RobotCommand Execute()
    {
        return new CollectEnergyCommand();
    }
}