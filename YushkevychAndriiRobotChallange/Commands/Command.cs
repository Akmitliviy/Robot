
using Robot.Common;

namespace YushkevychAndriiRobotChallange.Commands;

public abstract class Command
{
    protected YushkevychAndriiAlgorithm Algorithm;


    public void SetAlgorithm(YushkevychAndriiAlgorithm context)
    {
        this.Algorithm = context;
    }
    public abstract void Process();
    public abstract RobotCommand Execute();
}