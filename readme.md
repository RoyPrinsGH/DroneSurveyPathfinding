Observations:
- MC is almost as good as COBF at smaller paths (maxTicks <= 100), but quickly becomes too heavy to run
- COBF is great at longer paths, and is much faster than MC
- Greedy is not as bad as expected (usually ~90% of best)
- Random is worse than expected, with an average score per cell of around 25% of max across board
- Having the restore speed of a cell value too high will cause the algorithms to go around in circles, this is logically so: you won't need to explore for new paths if you can exploit the optimal path you've already taken.
- I tried a genetic algorithm, but it really wasn't performing well at all

Assumptions:
- I made the assumption that the grid is uniform in its value density, otherwise you might wanna try an algorithm that scans the grid and focuses on parts where the value density is higher.