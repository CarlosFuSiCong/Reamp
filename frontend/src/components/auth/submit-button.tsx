interface SubmitButtonProps {
  isLoading?: boolean;
  loadingText?: string;
  children: React.ReactNode;
}

export function SubmitButton({ isLoading, loadingText, children }: SubmitButtonProps) {
  return (
    <button
      type="submit"
      disabled={isLoading}
      className="flex w-full justify-center rounded-md border border-transparent bg-blue-600 px-4 py-2 text-sm font-medium text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 disabled:cursor-not-allowed disabled:bg-blue-400"
    >
      {isLoading ? loadingText : children}
    </button>
  );
}

