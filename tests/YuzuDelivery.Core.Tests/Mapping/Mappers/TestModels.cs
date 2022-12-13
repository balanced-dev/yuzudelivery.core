namespace YuzuDelivery.Core.Mapping;

class SourceA
{
    public int A { get; set; }
}

class DestA
{
    public int A { get; set; }
}

class SourceB
{
    public int B { get; set; }
}

class DestB
{
    public int B { get; set; }
}


class SourceContainerA
{
   public SourceA Item { get; set; }
}

class DestContainerB
{
    public DestB Item { get; set; }
}
