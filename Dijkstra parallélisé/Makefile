CC=gcc

FLAGS=-O3 -fopenmp

RM=rm -f

EXEC=dijkstra

SRC=opm_dij.c

all: $(EXEC)

$(EXEC):
	$(CC) $(FLAGS) $(SRC) -o $(EXEC)

run:
	./$(EXEC) 10000 100 42

clean:
	$(RM) $(EXEC)