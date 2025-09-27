interface UserRoleProps {
  role: string;
}

export default function UserRole({ role }: UserRoleProps) {
  const color = role === "Admin" ? "bg-red-500" : "bg-green-500";
  return (
    <span className={`${color} text-white px-2 py-1 rounded text-sm`}>
      {role}
    </span>
  );
}
