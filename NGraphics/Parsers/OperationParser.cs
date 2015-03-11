﻿using System;
using NGraphics.Codes;
using NGraphics.Models;

namespace NGraphics.Parsers
{
  public class OperationParser
  {
    public static Operation Parse(string operationString)
    {
      var operationChar = ParseOperationChar(operationString);

      return new Operation
      {
        OriginalValue = operationChar,
        Type = ParseType(operationChar)
      };
    }

    private static char ParseOperationChar(string operationString)
    {
      char operationChar;

      if (operationString.Length == 1)
      {
        operationChar = operationString[0];
      }
      else
      {
        operationChar = operationString.Substring(0, 1)[0];
      }

      return operationChar;
    }

    private static OperationType ParseType(char operation)
    {
      switch (char.ToUpper(operation))
      {
        case 'M':
          return OperationType.MoveTo;
        case 'L':
          return OperationType.LineTo;
        case 'C':
          return OperationType.CurveTo;
        case 'S':
          return OperationType.ContinueCurveTo;
        case 'A':
          return OperationType.ArcTo;
        case 'Z':
          return OperationType.Close;
        default:
          throw new NotSupportedException(string.Format("Operation {0} not supported", operation));
      }
    }
  }
}