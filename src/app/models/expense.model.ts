export interface Expense {
  id: number;
  title: string;
  amount: number;
  category: string;
  description?: string;
  createdAt: string;
  createdBy: string;
}

export interface ExpenseCreateRequest {
  title: string;
  amount: number;
  category: string;
  description?: string;
  createdBy: string;
}
