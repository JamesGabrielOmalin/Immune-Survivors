using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    static Random random = new Random();
    public static List<T> GenerateRandom<T>(this List<T> collection, int count)
    {
        return collection.OrderBy(d => random.Next()).Take(count).ToList();
    }
}