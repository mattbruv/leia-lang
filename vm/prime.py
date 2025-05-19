def is_prime(n):
    if n < 2:
        return False
    d = 2
    while d * d <= n:
        if n % d == 0:
            return False
        d += 1
    return True

def find_nth_prime(target_count):
    count = 0
    candidate = 2

    while True:
        if is_prime(candidate):
            count += 1
            if count == target_count:
                return candidate
        candidate += 1

# Compute and print the 461st prime number
nth = 46100
result = find_nth_prime(nth)
print(f"The {nth}th prime number is: {result}")
