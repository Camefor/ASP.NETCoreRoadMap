# C# 数值舍入与 ToString 格式化

## 1. 核心概念

C# 中经常遇到以下几种写法：

```csharp
value.ToString();
value.ToString("F0");
value.ToString("F2");

Math.Round(value);
Math.Round(value, 2);
Math.Round(value, MidpointRounding.AwayFromZero);
```

最重要的区别：

> **`Math.Round()` 是数值舍入；`ToString()` 是字符串格式化。**

可以简单记忆：

```text
Math.Round()
    ↓
得到一个“舍入后的数值”

ToString()
    ↓
得到一个“字符串”
```

---

# 2. `.ToString()`

## 作用

将数值按照默认的 `General` 通用格式转换成字符串。

```csharp
double value = 123.456;

value.ToString();
```

结果通常：

```text
"123.456"
```

它**不会主动要求保留几位小数**。

例如：

```csharp
123.456.ToString()   // "123.456"
123.4.ToString()     // "123.4"
123.0.ToString()     // "123"
```

因此：

```csharp
value.ToString()
```

不要理解成：

```csharp
Math.Round(value).ToString()
```

两者完全不是一回事。

---

# 3. `.ToString("F0")`

`F` = Fixed-point（固定小数格式）

`F0`：

> 固定显示 0 位小数。

例如：

```csharp
123.456.ToString("F0")
```

结果：

```text
"123"
```

```csharp
123.5.ToString("F0")
```

结果：

```text
"124"
```

因此 `F0` 会发生**舍入**。

### 常见用法

```csharp
value.ToString("F0"); // 0 位小数
value.ToString("F1"); // 1 位小数
value.ToString("F2"); // 2 位小数
value.ToString("F3"); // 3 位小数
```

例如：

```csharp
123.456.ToString("F0") // "123"
123.456.ToString("F1") // "123.5"
123.456.ToString("F2") // "123.46"
123.456.ToString("F3") // "123.456"
```

---

# 4. `.ToString("F2")`

表示：

> 固定显示 2 位小数。

例如：

```csharp
123.ToString("F2")
```

结果：

```text
"123.00"
```

```csharp
123.4.ToString("F2")
```

结果：

```text
"123.40"
```

```csharp
123.456.ToString("F2")
```

结果：

```text
"123.46"
```

因此：

```csharp
value.ToString("F2")
```

既会：

1. 舍入
2. 保留 2 位小数
3. 返回字符串

---

# 5. `Math.Round()`

## 基本作用

`Math.Round()` 用来对**数值进行舍入**。

```csharp
Math.Round(123.456)
```

结果：

```text
123
```

注意：

> 默认舍入到 **0 位小数，也就是整数**。

---

# 6. `Math.Round(value, decimals)`

第二个参数 `decimals`：

> 指定保留多少位小数。

例如：

```csharp
Math.Round(123.456, 0) // 123
Math.Round(123.456, 1) // 123.5
Math.Round(123.456, 2) // 123.46
Math.Round(123.456, 3) // 123.456
```

所以：

```csharp
Math.Round(value)
```

可以理解为：

```csharp
Math.Round(value, 0)
```

---

# 7. `Math.Round()` 默认是什么舍入规则？

C#：

```csharp
Math.Round(value)
```

默认使用：

```csharp
MidpointRounding.ToEven
```

也就是：

> **银行家舍入（Banker's Rounding）**

---

# 8. 银行家舍入 ToEven

普通情况下正常舍入。

特殊情况：

> **恰好 `.5` 时，选择最终结果中的偶数。**

例如：

```csharp
Math.Round(1.5) // 2
Math.Round(2.5) // 2
Math.Round(3.5) // 4
Math.Round(4.5) // 4
Math.Round(5.5) // 6
```

记忆：

```text
1.5 → 2
2.5 → 2
3.5 → 4
4.5 → 4
5.5 → 6
```

`.5` 时：

```text
1 和 2 → 选 2
2 和 3 → 选 2
3 和 4 → 选 4
4 和 5 → 选 4
```

也就是：

> **哪个结果是偶数，就选哪个。**

---

# 9. `MidpointRounding.AwayFromZero`

如果不希望使用银行家舍入，而希望：

> `.5` 永远远离 0

可以使用：

```csharp
Math.Round(value, MidpointRounding.AwayFromZero)
```

例如：

```csharp
Math.Round(1.5, MidpointRounding.AwayFromZero) // 2
Math.Round(2.5, MidpointRounding.AwayFromZero) // 3
Math.Round(3.5, MidpointRounding.AwayFromZero) // 4
```

负数：

```csharp
Math.Round(-1.5, MidpointRounding.AwayFromZero) // -2
Math.Round(-2.5, MidpointRounding.AwayFromZero) // -3
```

所以：

```text
ToEven：
  2.5 → 2
  4.5 → 4

AwayFromZero：
  2.5 → 3
  4.5 → 5
```

---

# 10. `.ToString("F0")` 和 `Math.Round()`

这一点非常重要。

```csharp
value.ToString("F0")
```

可以理解为：

> 格式化输出时，把数值舍入到 0 位小数。

而：

```csharp
Math.Round(value)
```

是：

> 直接得到一个舍入后的数值。

例如：

```csharp
double value = 123.456;

Math.Round(value)
```

得到：

```text
123
```

类型还是数值类型。

而：

```csharp
value.ToString("F0")
```

得到：

```text
"123"
```

类型是：

```csharp
string
```

---

# 11. 一个非常重要的区别

```csharp
Math.Round(value)
```

和：

```csharp
value.ToString("F0")
```

最终显示结果很多时候一样，但**过程和类型不同**。

### Math.Round

```text
123.456
   ↓
Math.Round()
   ↓
123
   ↓
数值
```

### ToString("F0")

```text
123.456
   ↓
ToString("F0")
   ↓
"123"
   ↓
字符串
```

---

# 12. `.ToString()` 和 `.ToString("F0")`

这两个非常容易混淆。

```csharp
123.456.ToString()
```

结果：

```text
"123.456"
```

而：

```csharp
123.456.ToString("F0")
```

结果：

```text
"123"
```

所以：

> `.ToString()` 不等于 `.ToString("F0")`

---

# 13. `.ToString("F2")` 与 `Math.Round(value, 2)`

例如：

```csharp
double value = 123.456;
```

### Math.Round

```csharp
Math.Round(value, 2)
```

得到：

```text
123.46
```

类型：

```text
double
```

### ToString

```csharp
value.ToString("F2")
```

得到：

```text
"123.46"
```

类型：

```text
string
```

所以：

> `Math.Round(value, 2)` 是数值计算  
> `ToString("F2")` 是格式化输出

---

# 14. 常见写法对比

| 写法 | 作用 | 结果类型 |
|---|---|---|
| `value.ToString()` | 默认格式转换 | `string` |
| `value.ToString("F0")` | 舍入并显示 0 位小数 | `string` |
| `value.ToString("F2")` | 舍入并显示 2 位小数 | `string` |
| `Math.Round(value)` | 舍入到整数 | 数值 |
| `Math.Round(value, 2)` | 舍入到 2 位小数 | 数值 |
| `Math.Round(value, MidpointRounding.ToEven)` | 银行家舍入到整数 | 数值 |
| `Math.Round(value, MidpointRounding.AwayFromZero)` | `.5` 远离 0 | 数值 |

---

# 15. 实际开发中的选择

## 只是为了显示

例如：

```csharp
price.ToString("F2")
```

适合：

- UI 显示
- 日志
- 文本输出
- API 返回字符串
- 报表

---

## 需要继续进行数学计算

使用：

```csharp
Math.Round(price, 2)
```

例如：

```csharp
decimal price = 12.345m;

var rounded = Math.Round(price, 2);

var result = rounded * 10;
```

因为 `rounded` 仍然是数值。

---

# 16. 你之前代码中的典型场景

### 写法一

```csharp
(item.Bids[0].Px * 100).ToString("F0")
```

含义：

```text
Px × 100
    ↓
舍入到整数
    ↓
转换为字符串
```

---

### 写法二

```csharp
Math.Round(item.Bids[0].Px * 100).ToString()
```

含义：

```text
Px × 100
    ↓
银行家舍入到整数
    ↓
转换为字符串
```

两者在“整数舍入 + 字符串输出”这个目的上可以认为是等价思路。

---

### 写法三

```csharp
(item.Bids[0].Vol * 100).ToString()
```

含义：

```text
Vol × 100
    ↓
不主动舍入
    ↓
直接转换为字符串
```

所以它**不能**和：

```csharp
Math.Round(item.Bids[0].Vol * 100).ToString()
```

等价。

---

# 17. 最重要的记忆口诀

```text
ToString()
    → 只是转换/格式化
    → 不指定小数位

ToString("F0")
    → 固定 0 位小数
    → 会舍入
    → 返回 string

ToString("F2")
    → 固定 2 位小数
    → 会舍入
    → 返回 string

Math.Round()
    → 数值舍入
    → 默认 0 位小数
    → 默认 ToEven（银行家舍入）

Math.Round(x, 2)
    → 数值舍入
    → 保留 2 位小数

Math.Round(x, ..., AwayFromZero)
    → .5 时远离 0
```

## 一句话总结

> **`Math.Round` 解决“数值应该变成多少”；`ToString("F0/F2")` 解决“这个数值应该怎样显示”。**