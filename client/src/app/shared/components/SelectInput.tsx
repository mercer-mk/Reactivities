import { FormControl, FormHelperText, InputLabel, MenuItem, Select } from "@mui/material";
 import  { SelectProps }from "@mui/material/Select";
 import { FieldValues, useController, UseControllerProps } from "react-hook-form"
 import { categoryOptions } from "../../../features/activities/form/categoryOptions";
 type Props<T extends FieldValues> = {
     items: {text: string, value: string}[];
     label: string;
 } & UseControllerProps<T> & SelectProps
 export default function SelectInput<T extends FieldValues>(props: Props<T>) {
     const {field, fieldState} = useController({...props});
 const items = categoryOptions;
     return (
         <FormControl fullWidth error={!!fieldState.error}>
             <InputLabel>{props.label}</InputLabel>
             <Select
                 value={field.value || ''}
                 label={props.label}
                 onChange={field.onChange}
             >
                 {items.map(item => (
                     <MenuItem key={item.value} value={item.value}>
                         {item.text}
                     </MenuItem>
                 ))}
             </Select>
             <FormHelperText>{fieldState.error?.message}</FormHelperText>
         </FormControl>   
     )
 }