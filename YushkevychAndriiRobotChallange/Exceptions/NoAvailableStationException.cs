﻿using System;

namespace YushkevychAndriiRobotChallange.Exceptions;

public class NoAvailableStationException : Exception
{
    public NoAvailableStationException()
    {}
    
    public NoAvailableStationException(string message) : base(message)
    {}
    
    public NoAvailableStationException(string message, Exception inner) : base(message, inner)
    {}
}