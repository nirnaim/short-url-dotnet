# Part 1
I chose this algorithm for URL shortening because because it combines the long URL with a salt and generates a hash, ensuring uniqueness in the resulting shortcode. 
The first 8 characters of the hash serve as the shortcode, maintaining a consistent length. 
In case of a collision, where the generated shortcode already exists, the algorithm increments the salt and repeats the process until a unique shortcode is obtained. 
This approach guarantees the uniqueness and consistency of shortcodes, making it suitable for URL shortening applications.

# Part 2
I chose The LRU (Least Recently Used) approach for implementing the memory cache layer because it efficiently manages limited memory resources by evicting the least recently accessed items when the cache reaches its capacity. 
This ensures that frequently accessed URLs remain in the cache, minimizing the need for database access and optimizing the system's performance during bursts of redirections. 
Additionally, LRU eviction policy provides predictable behavior and is well-suited for scenarios where the goal is to maximize cache hit rates while keeping memory usage under control.