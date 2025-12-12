import { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { motion } from 'framer-motion';
import {
  ArrowLeft,
  Pencil,
  Mail,
  Phone,
  Briefcase,
  User,
  Calendar,
  FileText,
  Clock,
  Loader2,
} from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Badge } from '@/components/ui/badge';
import { Skeleton } from '@/components/ui/skeleton';
import { Employee } from '@/types/employee';
import { employeeService } from '@/services/employeeService';
import { toast } from 'sonner';
import { format } from 'date-fns';
import { ptBR } from 'date-fns/locale';

export default function EmployeeDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [employee, setEmployee] = useState<Employee | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    if (id) {
      loadEmployee(Number(id));
    }
  }, [id]);

  const loadEmployee = async (employeeId: number) => {
    setIsLoading(true);
    try {
      const data = await employeeService.getById(employeeId);
      setEmployee(data);
    } catch (error) {
      toast.error('Erro ao carregar funcionário');
      navigate('/dashboard');
    } finally {
      setIsLoading(false);
    }
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return '-';
    return format(new Date(dateString), "dd 'de' MMMM 'de' yyyy", { locale: ptBR });
  };

  const InfoCard = ({
    icon: Icon,
    label,
    value,
  }: {
    icon: any;
    label: string;
    value: React.ReactNode;
  }) => (
    <div className="flex items-start gap-3 p-4 rounded-lg bg-muted/50">
      <div className="p-2 rounded-lg bg-primary/10 text-primary">
        <Icon className="h-5 w-5" />
      </div>
      <div>
        <p className="text-sm text-muted-foreground">{label}</p>
        <p className="font-medium text-foreground">{value || '-'}</p>
      </div>
    </div>
  );

  if (isLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center gap-4">
          <Skeleton className="h-10 w-10 rounded-lg" />
          <Skeleton className="h-8 w-48" />
        </div>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {Array.from({ length: 6 }).map((_, i) => (
            <Skeleton key={i} className="h-24 rounded-lg" />
          ))}
        </div>
      </div>
    );
  }

  if (!employee) {
    return null;
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      className="space-y-6"
    >
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div className="flex items-center gap-4">
          <Button variant="outline" size="icon" onClick={() => navigate(-1)}>
            <ArrowLeft className="h-5 w-5" />
          </Button>
          <div>
            <h1 className="text-3xl font-bold text-foreground">{employee.fullName}</h1>
            <div className="flex items-center gap-2 mt-1">
              {/*}
              <Badge variant={employee.isActive ? 'default' : 'secondary'}>
                {employee.isActive ? 'Ativo' : 'Inativo'}
              </Badge>
              */}
              <span className="text-muted-foreground">{employee.role?.name}</span>
            </div>
          </div>
        </div>
        <Button asChild>
          <Link to={`/employees/${employee.employeeId}/edit`}>
            <Pencil className="h-5 w-5" />
            Editar
          </Link>
        </Button>
      </div>

      {/* Info Cards */}
      <div className="bg-card rounded-xl border border-border p-6 shadow-sm">
        <h2 className="text-lg font-semibold mb-4">Informações Pessoais</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          <InfoCard icon={Mail} label="Email" value={employee.email} />
          <InfoCard icon={FileText} label="CPF" value={employee.docNumber} />
          <InfoCard icon={Calendar} label="Data de Nascimento" value={formatDate(employee.birthday)} />
          <InfoCard icon={Briefcase} label="Cargo" value={employee.role?.name} />
          <InfoCard icon={User} label="Gerente" value={employee.manager?.fullName} />
          {/*<InfoCard icon={Clock} label="Criado em" value={formatDate(employee.createdAt)} />*/}
        </div>
      </div>

      {/* Phone Numbers */}
      <div className="bg-card rounded-xl border border-border p-6 shadow-sm">
        <h2 className="text-lg font-semibold mb-4">Telefones</h2>
        {employee.phones && employee.phones.length > 0 ? (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {employee.phones.map((phone, index) => (
              <div key={phone || index} className="flex items-center gap-3 p-4 rounded-lg bg-muted/50">
                <div className="p-2 rounded-lg bg-primary/10 text-primary">
                  <Phone className="h-5 w-5" />
                </div>
                <div>
                  {/*<p className="text-sm text-muted-foreground">{phone.type}</p>*/}
                  <p className="font-medium text-foreground">{phone}</p>
                </div>
              </div>
            ))}
          </div>
        ) : (
          <p className="text-muted-foreground">Nenhum telefone cadastrado</p>
        )}
      </div>

      {/* Manager Info */}
      {employee.manager && (
        <div className="bg-card rounded-xl border border-border p-6 shadow-sm">
          <h2 className="text-lg font-semibold mb-4">Informações do Gerente</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <InfoCard icon={User} label="Nome" value={employee.manager.fullName} />
            <InfoCard icon={Mail} label="Email" value={employee.manager.email} />
          </div>
        </div>
      )}

      {/* Actions */}
      <div className="flex justify-end gap-3">
        <Button variant="outline" onClick={() => navigate(-1)}>
          Voltar
        </Button>
        <Button asChild>
          <Link to={`/employees/${employee.employeeId}/edit`}>
            <Pencil className="h-5 w-5" />
            Editar Funcionário
          </Link>
        </Button>
      </div>
    </motion.div>
  );
}
