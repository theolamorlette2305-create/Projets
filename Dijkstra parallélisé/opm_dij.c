#include <stdio.h>
#include <stdlib.h>
#include <limits.h>
#include <omp.h>

static unsigned long int next = 1;

int my_rand(void) {
    return ((next = next * 1103515245 + 12345) % ((u_long) RAND_MAX + 1));
}

void my_srand(unsigned int seed) {
    next = seed;
}

struct Graph {
    int nNodes;
    int *nEdges;
    int **edges;
    int **w;
};

struct Graph *createRandomGraph(int nNodes, int nEdges, int seed) {
    
    my_srand(seed);
    struct Graph *graph = (struct Graph *) malloc(sizeof(struct Graph));
    graph->nNodes = nNodes;
    graph->nEdges = (int *) malloc(sizeof(int) * nNodes);
    graph->edges = (int **) malloc(sizeof(int *) * nNodes);
    graph->w = (int **) malloc(sizeof(int *) * nNodes);

    int k, v;
    for (v = 0; v < nNodes; v++) {
        graph->edges[v] = (int *) malloc(sizeof(int) * nNodes);
        graph->w[v] = (int *) malloc(sizeof(int) * nNodes);
        graph->nEdges[v] = 0;
    }

    int source = 0;
    for (source = 0; source < nNodes; source++) {
        int nArestasVertice = (double) nEdges / nNodes * (0.5 + my_rand() / (double) RAND_MAX);
        for (k = nArestasVertice; k >= 0; k--) {
            int dest = my_rand() % nNodes;
            int w = 1 + (my_rand() % 10);
            graph->edges[source][graph->nEdges[source]] = dest;
            graph->w[source][graph->nEdges[source]++] = w;
        }
    }
    return graph;
}

int *dijkstra(struct Graph *graph, int source) {
    int nNodes = graph->nNodes;
    int *visited = (int *) malloc(sizeof(int) * nNodes);
    int *distances = (int *) malloc(sizeof(int) * nNodes);
    int k, v;

    int global_min_node = -1;
    int global_min_val = INT_MAX;

    #pragma omp parallel
    {
        #pragma omp for
        for (v = 0; v < nNodes; v++) {
            distances[v] = INT_MAX;
            visited[v] = 0;
        }
        
        #pragma omp single
        {
            distances[source] = 0;
            visited[source] = 1;
    
            for (k = 0; k < graph->nEdges[source]; k++)
                distances[graph->edges[source][k]] = graph->w[source][k];
        }
        
        for (int step = 1; step < nNodes; step++) {

            #pragma omp single
            {
                global_min_node = -1;
                global_min_val = INT_MAX;
            }
        
            int my_min_node = -1;
            int my_min_val = INT_MAX;

            #pragma omp for nowait
            for (int i = 0; i < nNodes; i++) {
                if (!visited[i] && distances[i] < my_min_val) {
                    my_min_val = distances[i];
                    my_min_node = i;
                }
            }

            #pragma omp critical
            {
                if (my_min_val < global_min_val) {
                    global_min_val = my_min_val;
                    global_min_node = my_min_node;
                }
            }

            #pragma omp barrier
            
            if (global_min_node == -1) {
                step = nNodes; 
            } else {

                #pragma omp single
                {
                    visited[global_min_node] = 1;
                }

                #pragma omp for schedule(static)
                for (int k = 0; k < graph->nEdges[global_min_node]; k++) {
                    int dest = graph->edges[global_min_node][k];
                    int weight = graph->w[global_min_node][k];
                    
                    int new_dist = global_min_val + weight;
                    if (new_dist < distances[dest]) {
                        distances[dest] = new_dist;
                    }
                }
            }
        } 
    }
    
    free(visited);
    return distances;
    
}

int main(int argc, char ** argv) {
    int nNodes;
    int nEdges;
    int seed;

    if (argc == 4) {
        nNodes = atoi(argv[1]);
        nEdges = atoi(argv[2]);
        seed = atoi(argv[3]);
    } else {
        fscanf(stdin, "%d %d %d", &nNodes, &nEdges, &seed);
    }

    nEdges = nNodes * nEdges;

    struct Graph *graph = createRandomGraph(nNodes, nEdges, seed);

    double start = omp_get_wtime();
    int *dist = dijkstra(graph, 0);
    double end = omp_get_wtime();

    double mean = 0;
    int count = 0; 

    for (int v = 0; v < graph->nNodes; v++) {
        if (dist[v] != INT_MAX) {
            mean += dist[v];
            count++;
        }
    }

    
    if (count > 0) {
        fprintf(stdout, "%.2f\n", mean / count);
        //fprintf(stderr, "Info: %d noeuds atteints sur %d\n", count, graph->nNodes);
    } else {
        fprintf(stdout, "0.00\n");
    }
    

    return 0;
}