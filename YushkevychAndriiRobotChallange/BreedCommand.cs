using Robot.Common;

namespace YushkevychAndriiRobotChallange;

public class BreedCommand : ICommand
{
    public int NewRobotEnergy = 100;
    public RobotCommand Execute()
    {
        return new CreateNewRobotCommand(){NewRobotEnergy = this.NewRobotEnergy};
    }
}