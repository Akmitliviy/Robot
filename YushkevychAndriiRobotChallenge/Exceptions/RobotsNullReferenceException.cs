using System;

namespace YushkevychAndriiRobotChallenge.Exceptions;

[Serializable]
public class RobotsNullReferenceException : Exception
{
    public RobotsNullReferenceException()
    {}

    public RobotsNullReferenceException(string message) : base(message)
    {}
    
    public RobotsNullReferenceException(string message, Exception innerException) : base(message, innerException)
    {}
}