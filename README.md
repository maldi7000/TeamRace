# TeamRace
Small project to determine the ranks for teams in a race with skiers and a tobogganist and print the results to a .pdf file using LaTeX.

## Input data
The input data is stored in a .csv file with the following structure:
```csv
Bib;  Name;               Category;   Team;               Time;   Gender
76;   Peter ® Porgrammer; 3;          Programmer Party;   40,02;  m
34;   Dave Designer;      3;          Designer Dudes;     40,22;  m
```

Every team has only one tobogganist, which is marked with a ® in the name.

## Team time computing
The time used to rank a normal team is the sum of the time of the tobogganist and the fastest two skiers. 
However, there is a mixed category where the time of at least one female and one male participant have to be considered in the total team time.

## Trophy ranking
There's also an additional ranking - the trophy ranking. The trophy time is the average time of all participants multiplied by 4 (the default team size).
The team which is closest to this trophy time wins the trophy ranking. 

**Note:** The time that is used to determine the difference to the trophy time is the sum of all participants, not the aforementioned team time.
