using System;
using System.Collections.Generic;

public interface IStackable<T>
{
    bool CanMerge(T other);
    void Merge(ref T other);
}

public interface IItemStorage
{
    int Insert(ItemStack stack);
    ItemStack Extract(Predicate<ItemStack> filter, int amount);
    IReadOnlyList<ItemStack> View();
}

public interface IFluidStorage
{
    float Fill(FluidStack stack);
    FluidStack Drain(Predicate<FluidStack> filter, float amount);
    IReadOnlyList<FluidStack> View();
}

public interface IFluidContainer
{
    float Capacity { get; }
    FluidStack CurrentFluid { get; }
    float Fill(FluidStack stack);
    FluidStack Drain(float amount);
    bool CanAccept(FluidStack stack);
}