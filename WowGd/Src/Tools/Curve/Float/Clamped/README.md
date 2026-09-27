This folder contains implementations of [floating-point curve samplers](..) with clamped output.

- [ClampedOutputSampler](#clampedoutputsampler)
  - [Usage](#usage)
  - [Implementation](#implementation)
- [LinearCurve](#linearcurve)
  - [Usage](#usage-1)
- [FastLogCurve](#fastlogcurve)
  - [Usage](#usage-2)
  - [Negative h](#negative-h)


# [ClampedOutputSampler](MATH_ClampedOutputSampler.cs)

The base definition of a clamped output sampler.

## Usage

Attach the curve sampler to a script, as shown in [curve](../..), and assign a **Min Output** and **Max Output**.
- **Min/Max Output** are respectively the minimum and maximum output value a call to `Sample()` method can yield.
  
  ![inspector-example](doc_resources/min_max.png)

## Implementation

To implement a concrete child of this class, you must implement the `GetRatio()` method.
```cs
protected abstract float GetRatio(float value);
```

This method takes an input passed to `Sample()`, and should return a value between 0 and 1.

The output of this function will be linearly mapped between the **Min** and **Max Output** when calling `Sample()`.  
So for a ratio of 0, the output will be **Min Output**, for 1, **Max OutPut**, and 0.5 will be **(Min Output + Max Ouptut) / 2**.

# [LinearCurve](MATH_LinearCurve.cs)

A simple clamped linear map.

## Usage

Simply give an **Min** and **Max Input** value.

For an input value of **Min Input**, the output value of `Sample()` will be **Min Output**, and vice-versa, and value in-between will be mapped linearly.

It is possible then to invert the evolution of the curve. For example, with -
- **Min/Max Input** - respectively 0/1
- **Min/Max Output** - respectively 1/0

Any value smaller or equal to zero will lead to an output of 1, and any value higher or equal to 1 will lead to an output of 0.



# [FastLogCurve](MATH_FastLogCurve.cs)

https://www.desmos.com/calculator/oqfxjchzwx?lang=fr

A log approximation map. It is not a strict logarithmic, as it is unecessarily expensive. The exact function is `x/x+h` where h is the "half life" of the value.

## Usage

Give a **Half Input** value, which corresponds to the input at which the ouput will be in the middle point between **Min** and **Max Output**.



For example
- **Min Output** is ``1``
- **Max Output** is ``2``
- **Half Input** `h` is such that `Sample(h)` = ``1.5``

The output value will evolve in a *pseudo-logarithmic* manner, from 0 to ∞.
| input | ratio output  | sample output                     |
|-------|---------------|-----------------------------------|
| 0     | 0             | 0                                 |
| `h`   | 0.5           | (`minOutput` + `maxOutput`) / 2   |
| ∞     | 1             | `maxOutput`                       |

> ⚠️ Note that the output will **never*** reach **Max Output**. It will only tend to it.  
> <small>*It technically can reach __max output__, but it will not if the input and Half Input have the same sign, which is the intended use.</small>

## Negative h

If **Half Input** is negative, then the input are also expected to be negative, and the lower the input, the closer the output to **Max Output**. (The input is still positive)

> ⚠️ **To be deprecated note** : This curve implementation might benefit a rework, or an extension for more flexibility.
> - The current implementation expects mostly that if h is positive, the inputs will be too, and vice-versa.
> - If h is negative/positive, and x is bigger/smaller than -h, the ratio is unexpectedly 1. That could easily be fixed with an additionnal check.
> - It would be nice to be able to change the scale of input values, for example, make so the evolution of the pseudo-logarithmic curve starts at a different value than 0.