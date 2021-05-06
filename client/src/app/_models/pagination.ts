export interface Pagination {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export class PaginatedResult<T> {
  result: T;
  pagination: Pagination;
}
/* {"totalCount":19,
"pageSize":2,
"totalPages":10,
"currentPage":1,
"PreviousPageLink":null,
"NextPageLink":null
}
 */
/* this is my paginations
  currentPage: number;
  itemsPerPage: number;
  totalItems: number;
  totalPages: number;
*/
