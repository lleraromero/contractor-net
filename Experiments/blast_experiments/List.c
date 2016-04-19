#include <stdlib.h>

typedef struct node {
  int data; 
  struct node *next;
} node;

typedef struct list {
  int size; 
  node* first;
} list;

list* l;

int invariant() {
  return l==0 || l->size >= 0;
}

int List() {
  l = (list*) malloc(sizeof(list));
  
  if (l == 0) return 0;
  
  l->size = 0; 
  l->first = 0;
  return 1;
}

int add_req() { 
  return l!=0; 
}

int add(int data) {
  node *tmp = l->first;
  while (tmp->next != l->first)
    tmp = tmp->next;
  tmp->next = (node*) malloc(sizeof(node));
  if(tmp->next == 0) {
    l = 0; 
    return 0;
  }
  tmp->next->data = data;
  tmp->next->next = l->first;
  l->size++;

  return 1;
}

int remove_req() {
  return l!=0 && l->size > 0;
}

void remove() {
  node* new_first = l->first->next;
  free(l->first);
  l->first = new_first;
}

int destroy_req() {
  return l!=0;
}

void destroy() {
  node* current;
  node* tmp;
  current = l->first;
  l->first = 0;
  while(current != 0) {
    tmp = current->next;
    free(current);
    current = tmp;
  }
  l = 0;
}
